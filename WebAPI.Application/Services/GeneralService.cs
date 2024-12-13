﻿using WebAPI.Domain.ExtensionMethods;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using WebAPI.Application.Generic;
using WebAPI.Domain.Constants;
using WebAPI.Domain.Interfaces.Services.Tools;
using WebAPI.Domain.Interfaces.Services;
using WebAPI.Domain.Cryptography;
using System.Text;

namespace WebAPI.Application.Services;

public class GeneralService : GenericService, IGeneralService
{
    private List<RefreshTokens> _refreshTokens = new List<RefreshTokens>();
    private readonly GeneralMethod _generalMethod;
    private EnvironmentVariables _environmentVariables { get; }

    public GeneralService(EnvironmentVariables environmentVariables, INotificationMessageService iNotificationMessageService) : base(iNotificationMessageService)
    {
        _environmentVariables = environmentVariables;
        _generalMethod = GeneralMethod.GetLoadExtensionMethods();
    }

    public string CreateJwtToken(Credentials credentials)
    {
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_environmentVariables.TokenSettings.Key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _environmentVariables.TokenSettings.Issuer,
            Audience = _environmentVariables.TokenSettings.Audience,
            Subject = new ClaimsIdentity(new Claim[]
            {
                    //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("Id",credentials.Id.ToString()),
                    new Claim(ClaimTypes.Name, credentials.Login.ToString()),
                    new Claim(ClaimTypes.Role, string.Join(",",credentials.Roles)), // são as permissões do usuario, onde podemos restringir os endpoints a partir da tag >>  No Authorize(Roles = "ROLE_AUDIT") por exemplo
                    new Claim("DateAccess", credentials.AccessDate.ToShortDateString()),
                    new Claim("TimeAccess", credentials.AccessDate.ToString("HH:mm:ss"))
            }),
            Expires = DateTime.UtcNow.AddSeconds(_environmentVariables.TokenSettings.Seconds),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenAuth = tokenHandler.WriteToken(token);
        tokenAuth = CryptographyTokenService.EncryptToken(tokenAuth, _environmentVariables.TokenSettings.Key);
        return tokenAuth;
    }

    public async Task<MemoryStream> Export2ZipAsync(string directory, EnumMemoryStreamFile typeFile = EnumMemoryStreamFile.PDF)
    {
        List<string> archives = new List<string>();
        var memoryStreamResult = _generalMethod.GetMemoryStreamType(typeFile);
        int count = 0;

        foreach (string arquivo in Directory.GetFiles(directory, $"*.{memoryStreamResult.Type}"))
        {
            archives.Add(arquivo);
        }

        using (MemoryStream ms = new MemoryStream())
        {
            using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, true))
            {
                foreach (string arquivo in archives)
                {
                    ZipArchiveEntry zipArchiveEntry = archive.CreateEntry($"file{count}.{memoryStreamResult.Extension}", CompressionLevel.Fastest);
                    using (var zipStream = zipArchiveEntry.Open()) zipStream.Write(System.IO.File.ReadAllBytes(arquivo), 0, System.IO.File.ReadAllBytes(arquivo).Length);
                    count++;
                }
            }
            await Task.CompletedTask;
            return ms;
        }
    }

    public async Task<bool> SendPushNotificationAsync(PushNotification notification, string tokenUser)
    {
        // Reference: http://codepickup.in/csharp/fcm-push-notification-in-csharp/
        // Se nao gerar a chave web api, so ir na autentication

        var postData = new
        {
            to = tokenUser,
            notification,
            // data = Algum dado em especial
        }.SerializeObject();

        Byte[] byteArray = Encoding.UTF8.GetBytes(postData.ToString());

        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(FixConstantsUrl.URL_TO_GET_FIREBASE);
            request.Method = "post";
            request.KeepAlive = false;
            request.ContentType = "application/json";
            request.Headers.Add($"Authorization: key={FixConstants.SERVER_API_KEY}");
            request.Headers.Add($"Sender: id={FixConstants.SENDER_ID}");
            request.ContentLength = byteArray.Length;

            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            WebResponse response = request.GetResponse();
            HttpStatusCode responseCode = ((HttpWebResponse)response).StatusCode;

            if (!responseCode.Equals(HttpStatusCode.OK))
                return false;

            StreamReader reader = new StreamReader(response.GetResponseStream());
            string responseLine = reader.ReadToEnd();
            reader.Close();

            await Task.CompletedTask;
            return true;

        }
        catch (Exception ex)
        {
            Notify(ex.Message);
            return false;
        }
    }

    public string GenerateToken(IEnumerable<Claim> claims)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_environmentVariables.TokenSettings.Key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var key = Encoding.ASCII.GetBytes(_environmentVariables.TokenSettings.Key);

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        if (!(securityToken is JwtSecurityToken jwtSecurityToken))
            throw new SecurityTokenException(FixConstants.ERROR_IN_REFRESHTOKEN);
        else if (!jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException(FixConstants.ERROR_IN_REFRESHTOKEN);

        return principal;
    }

    public void SaveRefreshToken(string username, string refreshToken)
    {
        _refreshTokens.Add(new RefreshTokens(username, refreshToken));
    }

    public string GetRefreshToken(string username)
    {
        return _refreshTokens.FirstOrDefault(x => x.Username == username).RefreshToken;
    }

    public void DeleteRefreshToken(string username, string refreshToken)
    {
        var item = _refreshTokens.FirstOrDefault(x => x.Username == username && x.RefreshToken == refreshToken);
        _refreshTokens.Remove(item);
    }

    public string ExtractObjectInformationsByReflection(object obj)
    {
        //obtem o tipo do objeto
        //esse tipo não tem relação com a instância de obj
        var tipo = obj.GetType();

        StringBuilder builder = new StringBuilder();
        //obtem o nome do tipo
        builder.AppendLine("Log do " + tipo.Name);
        builder.AppendLine("Data: " + DateOnlyExtensionMethods.GetDateTimeNowFromBrazil());

        //Vamos obter agora todas as propriedades do tipo
        //Usamos o método GetProperties para obter
        //o nome das propriedades do tipo
        foreach (var prop in tipo.GetProperties())
        {
            //usa a propriedade Name para obter o nome da propriedade
            //e o método GetValue() para obter o valor da instância desse tipo
            builder.AppendLine(prop.Name + ": " + prop.GetValue(obj));
        }

        return builder.ToString();
    }

    public bool ValidateToken(string jwtToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var validations = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = _environmentVariables.TokenSettings.Issuer,
            ValidAudience = _environmentVariables.TokenSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_environmentVariables.TokenSettings.Key))
        };

        if (GuardClauses.IsNullOrWhiteSpace(jwtToken))
            return false;

        var tokenData = handler.ValidateToken(jwtToken, validations, out var tokenSecure).Identity as ClaimsIdentity;
        bool tokenIsValid = tokenData != null ? true : false;
        return tokenIsValid;
    }

    public object ExtractDataToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var claims = jwtToken.Claims.ToList();

        //DadosParametrosTokenDTO dadosAutenticacaoTokenDTO = new();
        //dadosAutenticacaoTokenDTO.Id = claims[0]?.Value;
        //dadosAutenticacaoTokenDTO.UserNum = MetodosGeral.ObterValorCampoInteiro(claims[1]?.Value);
        //dadosAutenticacaoTokenDTO.Nome = claims[2]?.Value;
        //dadosAutenticacaoTokenDTO.PermissoesDetalhada = claims[3]?.Value;
        //dadosAutenticacaoTokenDTO.Servidor = MetodosGeral.ObterValorCampoInteiro(claims[4]?.Value);
        //dadosAutenticacaoTokenDTO.PermissaoDownloadDeSIDs = MetodosGeral.ObterValorCampoBooleano(claims[5]?.Value);
        //dadosAutenticacaoTokenDTO.DownloadLP = MetodosGeral.ObterValorCampoBooleano(claims[6]?.Value);
        //dadosAutenticacaoTokenDTO.DataValidade = MetodosGeral.ObterValorCampoDataHora(claims[7]?.Value);

        return default;
    }
}