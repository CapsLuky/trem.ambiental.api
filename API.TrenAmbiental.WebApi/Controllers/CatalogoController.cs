using System.Collections.Generic;
using System.Threading.Tasks;
using API.TrenAmbiental.Domain.Services.Interfaces;
using API.TrenAmbiental.DTO.DomainObjects;
using API.TrenAmbiental.DTO.Model.Catalogo;
using API.TrenAmbiental.DTO.ViewModel.Catalogo;
using API.TrenAmbiental.WebApi.Bases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.TrenAmbiental.WebApi.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CatalogoController : MainController
    {
        private static ICatalogoService _catalogoService;

        public CatalogoController(
            ICatalogoService catalogoService,
            INotificador notificador) : base(notificador)
        {
            _catalogoService = catalogoService;
        }

        [Authorize(Roles = "1")]
        [HttpPost("produto")]
        public ActionResult<bool> AdicionarProduto([FromBody] ProdutoModel produto)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var resultado = _catalogoService.AdicionarProduto(produto);

            return resultado ? CustomResponse() : CustomResponse(false);
        }

        [Authorize(Roles = "1")]
        [HttpPut("produto")]
        public async Task<ActionResult<bool>> AtualizarProduto([FromBody] ProdutoModel produto)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            return CustomResponse(await _catalogoService.AtualizarProduto(produto));
        }

        [Authorize(Roles = "1, 2, 3, 4")]
        [HttpGet("produto/{produtoId:int}")]
        public async Task<ActionResult<ProdutoViewModel>> ObterProdutoPorId(int produtoId)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            return CustomResponse(await _catalogoService.ObterProdutoPorId(produtoId));
        }

        [Authorize(Roles = "1, 2, 3, 4")]
        [HttpGet("produto/{canal}")]
        public async Task<ActionResult<List<ProdutoViewModel>>> ObterTodosProduto(string canal)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            return CustomResponse(await _catalogoService.ObterTodos(canal));
        }

        [Authorize(Roles = "1")]
        [HttpGet]
        [Route("produtos-atualizar-estoque")]
        public async Task<ActionResult<ProdutoViewModel>> AtualizarEstoque(int id)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);


            return CustomResponse(await _catalogoService.ObterProdutoPorId(id));
        }

        [Authorize(Roles = "1")]
        [HttpPost("produtos-atualizar-estoque")]
        public async Task<ActionResult<bool>> AtualizarEstoque(int id, int quantidade)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            bool resultado;

            if (quantidade > 0)
                resultado = await _catalogoService.ReporEstoque(id, quantidade);
            else
                resultado = await _catalogoService.DebitarEstoque(id, quantidade);

            return resultado ? CustomResponse() : CustomResponse(false);
        }

        [Authorize(Roles = "1")]
        [HttpGet("produto-tipo")]
        public async Task<ActionResult<List<ProdutoTipoViewModel>>> ProdutoTipo()
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var resultado = await _catalogoService.BuscarProdutoTipoList();

            return CustomResponse(resultado);
        }

        [Authorize(Roles = "1, 2")]
        [HttpPut("upload-foto")]
        public async Task<ActionResult<bool>> UploadFoto([FromBody] UploadImagemModel uploadImagemModel)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            return CustomResponse(await _catalogoService.UploadFoto(uploadImagemModel));
        }
    }
}
