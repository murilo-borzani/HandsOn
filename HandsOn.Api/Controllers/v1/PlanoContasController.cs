using Asp.Versioning;
using HandsOn.Regras;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HandsOn.Api.Controllers.v1;

[ApiController]
[Route("v{version:apiVersion}/planocontas")]
[ApiVersion("1.0")]
[Authorize]
public class PlanoContasController : ControllerBase
{
    readonly IPlanoContasServico _planoContasServico;

    public PlanoContasController(
        IPlanoContasServico planoContasServico)
    {
        _planoContasServico = planoContasServico ?? throw new ArgumentNullException(nameof(planoContasServico));

    }


    
}

