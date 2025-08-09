using HandsOn.Api.Controllers.v1;
using HandsOn.Api.Models;
using HandsOn.Dto;
using HandsOn.Regras;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HandsOn.Api.Tests.Controllers.v1;

public class SegurancaControllerTests
{
    private readonly Mock<ISegurancaServico> _servicoMock;
    private readonly SegurancaController _controller;

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

    public SegurancaControllerTests()
    {
        _servicoMock = new Mock<ISegurancaServico>();
        _controller = new SegurancaController(_servicoMock.Object);
    }

    [Fact]
    public async Task Token_BadRequest_ClienteNulo()
    {
        var resposta = await _controller.Token(null);

        var badRequest = Assert.IsType<BadRequestObjectResult>(resposta.Result);
        Assert.Equal("Cliente não pode ser nulo", ((RetornoErro)badRequest.Value).Erro);
    }

    [Fact]
    public async Task Token_BadRequest_GrantTypeNulo()
    {
        var cliente = new ClienteApi { Tipo = null, Id = "id", Secret = "secret" };

        var resposta = await _controller.Token(cliente);

        var badRequest = Assert.IsType<BadRequestObjectResult>(resposta.Result);
        Assert.Equal("Informe grant_type", ((RetornoErro)badRequest.Value).Erro);
    }

    [Fact]
    public async Task Token_BadRequest_ClientIdNulo()
    {
        var cliente = new ClienteApi { Tipo = "tipo", Id = null, Secret = "secret" };

        var resposta = await _controller.Token(cliente);

        var badRequest = Assert.IsType<BadRequestObjectResult>(resposta.Result);
        Assert.Equal("Informe client_id", ((RetornoErro)badRequest.Value).Erro);
    }

    [Fact]
    public async Task Token_BadRequest_ClientSecretNulo()
    {
        var cliente = new ClienteApi { Tipo = "tipo", Id = "id", Secret = null };

        var resposta = await _controller.Token(cliente);

        var badRequest = Assert.IsType<BadRequestObjectResult>(resposta.Result);
        Assert.Equal("Informe client_secret", ((RetornoErro)badRequest.Value).Erro);
    }

    [Fact]
    public async Task Token_Unauthorized_ValidaCredenciais_RetornaNulo()
    {
        var cliente = new ClienteApi { Tipo = "tipo", Id = "id", Secret = "secret" };
        _servicoMock.Setup(s => s.ValidaCredenciais("id", "secret")).ReturnsAsync((UsuarioDto)null);

        var resposta = await _controller.Token(cliente);

        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(resposta.Result);
        Assert.Equal("Credenciais inválidas", ((RetornoErro)unauthorized.Value).Erro);
    }

    [Fact]
    public async Task Token_Unauthorized_GeraToken_NaoRetornaToken()
    {
        var cliente = new ClienteApi { Tipo = "tipo", Id = "id", Secret = "secret" };
        var usuario = new UsuarioDto { Id = 1, Nome = "Test", ClientId = "id", ClientSecret = "secret" };
        _servicoMock.Setup(s => s.ValidaCredenciais("id", "secret")).ReturnsAsync(usuario);
        _servicoMock.Setup(s => s.GeraToken(usuario)).Returns(("", 3600));

        var resposta = await _controller.Token(cliente);
        
        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(resposta.Result);
        Assert.Equal("Credenciais inválidas", ((RetornoErro)unauthorized.Value).Erro);
    }

    [Fact]
    public async Task Token_Ok_TokenGerado()
    {
        var cliente = new ClienteApi { Tipo = "tipo", Id = "id", Secret = "secret" };
        var usuario = new UsuarioDto { Id = 1, Nome = "Test", ClientId = "id", ClientSecret = "secret" };
        _servicoMock.Setup(s => s.ValidaCredenciais("id", "secret")).ReturnsAsync(usuario);
        _servicoMock.Setup(s => s.GeraToken(usuario)).Returns(("token123", 3600));

        var resposta = await _controller.Token(cliente);

        var okResult = Assert.IsType<OkObjectResult>(resposta.Result);
        var token = Assert.IsType<ClienteToken>(okResult.Value);
        Assert.Equal("Bearer", token.Tipo);
        Assert.Equal("token123", token.Token);
        Assert.Equal(3600, token.Expiracao);
    }
}