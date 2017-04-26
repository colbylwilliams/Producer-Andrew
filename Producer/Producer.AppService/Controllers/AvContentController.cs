using NomadCode.Azure.Controllers;

using Producer.AppService.Models;
using Producer.Domain;

namespace Producer.AppService.Controllers
{
    public class AvContentController : AzureEntityController<AvContent, ProducerContext>
    {
    }
}
