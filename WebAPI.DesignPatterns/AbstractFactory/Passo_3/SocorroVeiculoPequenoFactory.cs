﻿using WebAPI.DesignPatterns.AbstractFactory.Passo_1;
using WebAPI.DesignPatterns.AbstractFactory.Passo_2;

namespace WebAPI.DesignPatterns.AbstractFactory.Passo_3;

// Concrete Factory
public class SocorroVeiculoPequenoFactory : AutoSocorroFactory
{
    public override Guincho CriarGuincho()
    {
        return GuinchoCreator.Criar(Porte.Pequeno);
    }

    public override Veiculo CriarVeiculo(string modelo, Porte porte)
    {
        return VeiculoCreator.Criar(modelo, porte);
    }
}