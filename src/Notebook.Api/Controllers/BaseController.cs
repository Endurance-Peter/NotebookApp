using Microsoft.AspNetCore.Mvc;
using Notebook.Infrastructure.UnitOfWorks;

namespace Notebook.Api.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        public BaseController(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public IUnitOfWork UnitOfWork { get; }
    }
}
