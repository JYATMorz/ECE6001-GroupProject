﻿using Microsoft.EntityFrameworkCore;

namespace GameFellowship.Data.Database;

public class GameFellowshipDb : DbContext
{
	public DbSet<User> Users { get; set; }
	public DbSet<Game> Games { get; set; }
	public DbSet<Post> Posts { get; set; }

	public GameFellowshipDb(DbContextOptions<GameFellowshipDb> options) : base(options)
	{
		// TODO: Maybe ...
	}
}
