using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData;
using System.Net;
using Wilczura.Demo.Common;
using Wilczura.Demo.Common.Exceptions;
using Wilczura.Demo.Persistence.Repositories;

namespace Wilczura.Demo.Host.OData;

// TODO: Odata Group Name for swagger
[ApiExplorerSettings(GroupName = "odata")]
public class CustomODataController<TEntity> : ODataController
    where TEntity: class, IGetId, new()
{
    protected readonly ODataRepository<TEntity> _repository;

    public CustomODataController(ODataRepository<TEntity> repository)
    {
        _repository = repository;
    }

    [EnableQuery]
    public IActionResult Get(CancellationToken token)
    {
        return Ok(_repository.Get());
    }

    [EnableQuery]
    public IActionResult Get([FromRoute] int key, CancellationToken token)
    {
        try
        {
            var item = _repository.Get(key)
                .SingleOrDefault();

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }
        catch (Exception ex)
        {
            return GetErrorResult(ex);
        }
    }

    protected virtual IActionResult GetErrorResult(Exception ex)
    {
        // TODO: handle error for odata response
        var error = new ODataError()
        {
            Details = [
                new ODataErrorDetail()
                    {
                        ErrorCode = "500",
                        Message= "test",
                        Target = "Abc_Xyz"
                    }
            ],
            Message = ex.Message,
            ErrorCode = ((int)HttpStatusCode.BadRequest).ToString()
        };
        return ODataErrorResult(error);
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] TEntity entity, CancellationToken token)
    {
        try
        {
            ValidateEntity(entity, HttpMethod.Post);

            if (entity.GetId() != 0)
            {
                return BadRequest("Entity Id non-zero. Use PUT or PATCH instead.");
            }

            var newEntity = await _repository.UpsertAsync(entity);

            if (newEntity == null)
            {
                throw new CommonException("Processing return unexpected result.");
            }

            return Created(newEntity);
        }
        catch (Exception ex)
        {
            return GetErrorResult(ex);
        }
    }

    [HttpPut]
    public async Task<IActionResult> PutAsync(long key, [FromBody] Delta<TEntity> entity, CancellationToken token)
    {
        try
        {
            var originalEntity = _repository.Get(key)
                .FirstOrDefault();

            if (originalEntity == null)
            {
                return NotFound($"Not found {typeof(TEntity)} with id = {key}");
            }

            entity.Put(originalEntity);

            ValidateEntity(originalEntity, HttpMethod.Put);

            if (originalEntity.GetId() != key)
            {
                return BadRequest("Entity Id is different than Id from route.");
            }

            var editedEntity = await _repository.UpsertAsync(originalEntity);
            if (editedEntity == null)
            {
                throw new CommonException("Processing return unexpected result.");
            }

            return Updated(editedEntity);
        }
        catch (Exception ex)
        {
            return GetErrorResult(ex);
        }
    }

    [HttpPatch]
    public async Task<IActionResult> PatchAsync(int key, Delta<TEntity> entity, CancellationToken token)
    {
        try
        {
            var originalEntity = _repository.Get(key)
                .FirstOrDefault();

            if (originalEntity == null)
            {
                return NotFound($"Not found {typeof(TEntity)} with id = {key}");
            }

            entity.Patch(originalEntity);

            ValidateEntity(originalEntity, HttpMethod.Patch);

            if (originalEntity.GetId() != key)
            {
                return BadRequest("Entity Id is different than Id from route.");
            }

            var editedEntity = await _repository.UpsertAsync(originalEntity);
            if (editedEntity == null)
            {
                throw new CommonException("Processing return unexpected result.");
            }

            return Updated(editedEntity);
        }
        catch (Exception ex)
        {
            return GetErrorResult(ex);
        }
    }

    protected virtual void ValidateEntity(TEntity entity, HttpMethod httpMethod)
    {
    }
}
