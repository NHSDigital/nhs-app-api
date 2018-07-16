﻿using System;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision
{
    public class VisionTokenValidationService : ITokenValidationService
    {
        public bool IsValidConnectionTokenFormat(string connectionToken)
        {
            if (string.IsNullOrEmpty(connectionToken))
            {
                return false;
            }

            try
            {
                var token = connectionToken.DeserializeJson<Im1ConnectionToken>();
                return !string.IsNullOrEmpty(token?.RosuAccountId) && !string.IsNullOrEmpty(token.ApiKey);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}