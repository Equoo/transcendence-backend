namespace KeepGrouped.API.Users;

/// <summary>A freshly minted token pair, handed back by a handler for the endpoint to put in cookies.</summary>
public sealed record IssuedTokens(string AccessToken, string RefreshToken);
