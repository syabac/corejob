using CoreJob.App.Models;
using CoreJob.Library.Ticketing.Repository;

using Microsoft.AspNetCore.Mvc;

using System;

namespace CoreJob.App.Controllers
{
	[Route("ticket")]
	[ApiController]
	public class TicketController : ControllerBase
	{
		private TicketRepository ticketRepository = new();
		private TicketParameterRepository parameterRepository = new();

		[HttpPost]
		public IActionResult Create([FromBody] TicketRequest request)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var id = ticketRepository.Create(request.TicketName, request.Parameters);

			return Created(Url.Content("~/ticket/" + id), id);
		}

		[HttpGet("{ticketId}")]
		public IActionResult Find(Guid? ticketId)
		{
			var ticket = ticketRepository.FindById(ticketId);

			if (ticket == null)
				return NotFound(new { message = "TicketId is not valid" });

			return Ok(new
			{
				Detail = ticket,
				Parameters = parameterRepository.GetParametersByTicketId((Guid)ticket.Id)
			});
		}
	}
}
