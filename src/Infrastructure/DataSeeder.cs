using System;
using System.Collections.Generic;
using System.Linq;
using Conduit.Domain;
using Conduit.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

namespace Conduit.Infrastructure
{
public class DataSeeder
{

    public static void Seed(ConduitContext context, IPasswordHasher hasher)
    {
        // insure the database creation
        context.Database.EnsureCreated();

        // add admin
        if (!context.Persons.Any())
        {
            var salt = Guid.NewGuid().ToByteArray();
            var hash =  hasher.Hash("demo", salt);

            var admin = new Person {
                Bio = "Administrator",
                Email = "demo@demo.com",
                Hash = hash,
                Salt = salt,
                Username = "demo"
            };

            context.Add(admin);
            context.SaveChanges();
        }

        // add admin's article
        if (!context.Articles.Any()) {
             var author = context.Persons.First(x => x.Username == "demo");
             var tags = new List<Tag>{ 
                 new Tag { TagId = "jeremiah"},
                 new Tag { TagId = "new covenant"}
                 };
             // add only if no tags exist
             if (!context.Tags.Any()) {
                 context.Tags.AddRange(tags);
                 context.SaveChanges();
             }
                var title = "God of Israel New Covenant";
                var article = new Article()
                {
                    Author = author,
                    Body = @"“Behold, the days are coming, declares the LORD, when I will make a new covenant with the house of Israel and the house of Judah, not like the covenant that I made with their fathers on the day when I took them by the hand to bring them out of the land of Egypt, my covenant that they broke, though I was their husband, declares the LORD. For this is the covenant that I will make with the house of Israel after those days, declares the LORD: I will put my law within them, and I will write it on their hearts. And I will be their God, and they shall be my people. And no longer shall each one teach his neighbor and each his brother, saying, ‘Know the LORD,’ for they shall all know me, from the least of them to the greatest, declares the LORD. For I will forgive their iniquity, and I will remember their sin no more.” Thus says the LORD, who gives the sun for light by day and the fixed order of the moon and the stars for light by night, who stirs up the sea so that its waves roar— the LORD of hosts is his name: “If this fixed order departs from before me, declares the LORD, then shall the offspring of Israel cease from being a nation before me forever.” Thus says the LORD: “If the heavens above can be measured, and the foundations of the earth below can be explored, then I will cast off all the offspring of Israel for all that they have done, declares the LORD.” “Behold, the days are coming, declares the LORD, when the city shall be rebuilt for the LORD from the Tower of Hananel to the Corner Gate. And the measuring line shall go out farther, straight to the hill Gareb, and shall then turn to Goah. The whole valley of the dead bodies and the ashes, and all the fields as far as the brook Kidron, to the corner of the Horse Gate toward the east, shall be sacred to the LORD. It shall not be plucked up or overthrown anymore forever.”(Jer 31:31-40)",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Description = "New Covenant",
                    Title = title,
                    Slug = title.GenerateSlug()
                };
                context.Articles.Add(article);
                context.ArticleTags.AddRange(tags.Select(x => new ArticleTag()
                {
                    Article = article,
                    Tag = x
                }));

                context.SaveChanges();
        }
    }
}
}