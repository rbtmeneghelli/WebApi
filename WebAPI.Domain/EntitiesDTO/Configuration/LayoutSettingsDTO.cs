﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Domain.Generic;

namespace WebAPI.Domain.EntitiesDTO.Configuration
{
    public record LayoutSettingsDTO : GenericDTO
    {
        [Required]
        [Display(Name = "LogoWeb", Description = "Logo da aplicação Web")]
        public IFormFile LogoWeb { get; set; }

        [Required]
        [Display(Name = "BannerWeb", Description = "Banner da aplicação Web")]
        public IFormFile BannerWeb { get; set; }

        [Required]
        [Display(Name = "LogoMobile", Description = "Logo da aplicação Mobile")]
        public IFormFile LogoMobile { get; set; }

        [Required]
        [Display(Name = "BannerMobile", Description = "Banner da aplicação Mobile")]
        public IFormFile BannerMobile { get; set; }
    }
}
