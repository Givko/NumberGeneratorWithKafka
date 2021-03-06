﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Contracts
{
    public interface ILogger
    {
        void LogInfo(string message);
        void LogException(Exception ex);
    }
}
