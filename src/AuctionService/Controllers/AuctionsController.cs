using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]

public class AuctionsController:ControllerBase
{
    private readonly AuctionDBContext _context;
    private readonly IMapper _mapper;
    public AuctionsController(AuctionDBContext context,IMapper mapper)
    {
        _context= context;
        _mapper= mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuctionDTO>>> GetAllAuctions()
    {
        var auctions = await _context.Auctions.Include(x=>x.Item).OrderBy(x=>x.Item.Make).ToListAsync();
        return _mapper.Map<List<AuctionDTO>> (auctions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDTO>> GetAuctionbyID(Guid id)
    {
        var auction = await _context.Auctions.Include(x=>x.Item).FirstOrDefaultAsync(x => x.Id==id);
        if(auction==null)
        {
            return NotFound();
        }
        return _mapper.Map<AuctionDTO> (auction);
    }

     [HttpPost]
    public async Task<ActionResult<AuctionDTO>> CreateAuction(CreateAuctionDTO auctionDTO)
    {
        var auction = _mapper.Map<Auction>(auctionDTO);
        _context.Auctions.Add(auction);
        var result = await _context.SaveChangesAsync()>0;

        if(!result)
        {
            return BadRequest("unable to save");

           
        }
         return CreatedAtAction(nameof(GetAuctionbyID),new{auction.Id},_mapper.Map<AuctionDTO>(auction));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<AuctionDTO>> UpdateAuction(Guid id,UpdateAuctionDTO updateAuctionDTO)
    {
        var auction = await _context.Auctions.Include(x=>x.Item).FirstOrDefaultAsync(x => x.Id==id);
        if(auction==null)
        {
            return NotFound();
        }

        
        auction.Item.Make= updateAuctionDTO.Make ?? auction.Item.Make;
        auction.Item.Color= updateAuctionDTO.Color ?? auction.Item.Color;
        auction.Item.Model= updateAuctionDTO.Model ?? auction.Item.Model;
        auction.Item.Mileage= updateAuctionDTO.Mileage ?? auction.Item.Mileage;
        auction.Item.Year= updateAuctionDTO.Year ?? auction.Item.Year;
        var result = await _context.SaveChangesAsync() >0;
        if(result) 
        {
            return Ok();
        }
        return BadRequest("unable to save");
    }

}