﻿using System.Net;

namespace MagicVilla_Web.Models
{
    public class APIResponse
    {
        public APIResponse() {
            ErrorMessages = new List<string>();
        }
        public HttpStatusCode StatusCode { get; set; }
        public bool isSuccess { get; set; } = true;
        public List<string> ErrorMessages { get; set; }
        public object Result { get; set; }
    }
}
