﻿using System.Threading.Tasks;
using CoMon.Configuration.Dto;

namespace CoMon.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}