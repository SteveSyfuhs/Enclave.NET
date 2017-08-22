using Enclave.NET.Http.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Enclave.NET.Http.Controllers
{
    [Route("keys")]
    public class KeysController : ControllerBase
    {
        private readonly IEnclaveService enclave;

        public KeysController(IEnclaveService enclave)
        {
            this.enclave = enclave;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var keys = await enclave.ListKeys();

            return Ok(keys);
        }

        [HttpPost("{id}/encrypt")]
        public async Task<IActionResult> Encrypt(string id, [FromBody] EncryptRequest request)
        {
            var encrypted = await enclave.Encrypt(enclave.ParseKeyId(id), request.Value);

            return Ok(encrypted);
        }

        [HttpPost("generate")]
        public async Task<IActionResult> Generate([FromBody] GenerateRequest request)
        {
            var generated = await enclave.GenerateKey(request.KeyType);

            return Ok(generated);
        }

        [HttpPost("{id}/decrypt")]
        public async Task<IActionResult> Decrypt(string id, [FromBody] DecryptRequest request)
        {
            var decrypted = await enclave.Decrypt<object>(enclave.ParseKeyId(id), request.Value);

            return Ok(new DecryptResult { Value = decrypted.Serialize() });
        }

        [HttpPost("{id}/authenticate")]
        public async Task<IActionResult> Authenticate(string id, [FromBody] AuthenticateRequest request)
        {
            var identity = await enclave.AuthenticateToken(KeyIdentifier.Parse(id), request.Scheme, request.Token);

            if (identity == null)
            {
                return Unauthorized();
            }

            return Ok(new AuthenticateResult { Identity = identity.Serialize() });
        }

        [HttpPost("{id}/sign")]
        public async Task<ActionResult> Sign(string id, [FromBody] SignRequest request)
        {
            var signed = await enclave.Sign(enclave.ParseKeyId(id), request.Value);

            return Ok(new SignResult { Value = signed });
        }

        [HttpPost("{id}/validate")]
        public async Task<ActionResult> Validate(string id, [FromBody] ValidateRequest request)
        {
            var signed = await enclave.Validate(enclave.ParseKeyId(id), request.Value);

            return Ok(new ValidateResult { Result = signed });
        }
    }
}