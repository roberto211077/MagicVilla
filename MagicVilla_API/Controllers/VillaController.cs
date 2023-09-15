using AutoMapper;
using MagicVilla_API.Datos;
using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.Dto;
using MagicVilla_API.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        private readonly ILogger<VillaController> _logger;
        //private readonly ApplicationDbContext _db;
        private readonly IVillaRepositorio _villaRepo;
        private readonly IMapper _mapper;
        protected APIResponse _response;

        //Constructor donde inyectamos los servicios
        public VillaController(ILogger<VillaController> logger, ApplicationDbContext db, IMapper mapper, IVillaRepositorio villaRepo)
        {

            _logger = logger;
            // _db = db;
            _villaRepo = villaRepo;
            _mapper = mapper;
            _response = new();

        }


        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillas() 
        {
            try
            {
                _logger.LogInformation("Obtener las Villas");

                IEnumerable<Villa> villalist = await _villaRepo.ObtenerTodos();

                _response.Resultado = _mapper.Map<IEnumerable<VillaDto>>(villalist);
                _response.statusCode = HttpStatusCode.OK;

                return Ok(_response);

            }
            catch (Exception ex)
            {

                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return Ok(_response);
        }



        [HttpGet("{id:int}", Name ="GetVilla")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _logger.LogError("Error al traer Villa con Id " + id);
                    _response.statusCode = HttpStatusCode.BadRequest;
                    _response.IsExitoso = false;
                    return BadRequest(_response);
                }

                // var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
                var villa = await _villaRepo.Obtener(x => x.Id == id);

                if (villa == null)
                {
                    _response.statusCode = HttpStatusCode.NotFound;
                    _response.IsExitoso = false;
                    return NotFound(_response);
                }

                _response.Resultado = _mapper.Map<VillaDto>(villa);
                _response.statusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex)
            {

                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _response;
           
        }


        [HttpPost]
        [Authorize(Roles ="admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CrearVilla([FromBody] VillaCreateDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                //Model State personalizado
                if (await _villaRepo.Obtener(v => v.Nombre.ToLower() == createDto.Nombre.ToLower()) != null)
                {
                    ModelState.AddModelError("ErrorMessages", "La villa con ese nombre ya existe");
                    return BadRequest(ModelState);
                }


                if (createDto == null)
                {
                    return BadRequest(createDto);
                }

                Villa modelo = _mapper.Map<Villa>(createDto);

                modelo.FechaCreacion = DateTime.Now;
                modelo.FechaActualizacion = DateTime.Now;

                await _villaRepo.Crear(modelo);
                _response.Resultado = modelo;
                _response.statusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetVilla", new { id = modelo.Id }, _response);
            }
            catch (Exception ex)
            {

                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;
        }


        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteVilla(int id) 
        {
            try
            {
                if (id == 0)
                {
                    _response.IsExitoso = false;
                    _response.statusCode = HttpStatusCode.BadRequest;

                    return BadRequest(_response);

                }

                var villa = await _villaRepo.Obtener(v => v.Id == id);

                if (villa == null)
                {
                    _response.IsExitoso = false;
                    _response.statusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                // VillaStore.villaList.Remove(villa);

                //El Remove no es un metodo asyncrono
                await _villaRepo.Remover(villa);

                _response.statusCode = HttpStatusCode.NoContent;

                return Ok(_response);
            }
            catch (Exception ex)
            {

                _response.IsExitoso=false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return BadRequest(_response);


            
        }


        [HttpPut("{id:int}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDto updateDto) 
        {
            if (updateDto == null || id != updateDto.Id)
            {
                _response.IsExitoso = false;
                _response.statusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
            
            Villa modelo = _mapper.Map<Villa>(updateDto);
           
            //Update no es un metodo asyncrono
            await _villaRepo.Actualizar(modelo);
            _response.statusCode = HttpStatusCode.NoContent;
           
            return Ok(_response);

        }


        [HttpPatch("{id:int}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
        {
            if (patchDto == null || id == 0)
            {
                return BadRequest();
            }

            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);

            //AsNoTracking Permite consultar un registro del dbContext pero que no se trake,que no quede abierto

            var villa = await _villaRepo.Obtener(v => v.Id == id, tracked:false);

            VillaUpdateDto villaDto = _mapper.Map<VillaUpdateDto>(villa);

            
            if(villa == null) return BadRequest();

            patchDto.ApplyTo(villaDto, ModelState);

            if(!ModelState.IsValid) 
            {
                return BadRequest(ModelState);
            }

            Villa modelo = _mapper.Map<Villa>(villaDto);

           
            await _villaRepo.Actualizar(modelo);
            _response.statusCode = HttpStatusCode.NoContent;

            return Ok(_response);

        }
    }
    
}
