﻿namespace BlazingPizza.Server;

public class OidcConfigurationController : Controller
{
    public OidcConfigurationController(IClientRequestParametersProvider clientRequestParametersProvider) =>
        ClientRequestParametersProvider = clientRequestParametersProvider;

    public IClientRequestParametersProvider ClientRequestParametersProvider { get; }

    [HttpGet("_configuration/{clientId}")]
    public IActionResult GetClientRequestParameters([FromRoute] string clientId)
    {
        var parameters = ClientRequestParametersProvider.GetClientParameters(HttpContext, clientId);
        return Ok(parameters);
    }
}
