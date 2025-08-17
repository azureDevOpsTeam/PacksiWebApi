﻿using ApplicationLayer.Extensions.SmartEnums;

namespace ApplicationLayer.DTOs
{
    public class AuthorizeResultDto
    {
        public RequestStatus RequestStatus { get; set; }

        public string Message { get; set; }

        public string UserFullName { get; set; }

        public string AccessTokens { get; set; }

        public string RefreshToken { get; set; }
    }

    public class AuthorizeTokenResultViewModel
    {
        public string AccessTokens { get; set; }

        public string RefreshToken { get; set; }
    }
}