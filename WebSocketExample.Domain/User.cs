﻿namespace WebSocketExample.Domain;

public sealed class User(string username)
{
    public string Username { get; } = username;
}