﻿using MomoAPI.Log;

namespace MomoAPI.Net.Config;

public class ClientConfig
{
    public string Host { get; init; }

    public int Port { get; init; }

    public string AccessToken { get; init; }

    public LogWriter Log { get; init; }
}
