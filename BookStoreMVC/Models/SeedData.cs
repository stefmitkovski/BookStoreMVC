using BookStoreMVC.Areas.Identity.Data;
using BookStoreMVC.Data;
using BookStoreMVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;

namespace MVCMovie.Models
{
    public class SeedData
    {
        public static async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<BookStoreMVCUser>>();
            IdentityResult roleResult;
            //Add Admin Role
            var roleAdminCheck = await RoleManager.RoleExistsAsync("Admin");
            if (!roleAdminCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin")); }
            //Add User Role
            var roleUserCheck = await RoleManager.RoleExistsAsync("User");
            if (!roleUserCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("User")); }

            BookStoreMVCUser user = await UserManager.FindByEmailAsync("admin@books.com");
            if (user == null)
            {
                var User = new BookStoreMVCUser();
                User.Email = "admin@books.com";
                User.UserName = "admin@books.com";
                string userPWD = "Admin123";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                //Add default User to Role Admin      
                if (chkUser.Succeeded) { var result1 = await UserManager.AddToRoleAsync(User, "Admin"); }
            }
        }
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new BookStoreMVCContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<BookStoreMVCContext>>()))
            {
                CreateUserRoles(serviceProvider).Wait();
                // Look for any book.
                if (context.Author.Any() || context.Book.Any() || context.bookGenres.Any() || context.Genre.Any() || context.Review.Any() || context.userBooks.Any())
                {
                    return;   // DB has been seeded
                }


                context.Author.AddRange(
                    new Author { /*Id = 1, */FirstName = "Leo", LastName = "Tolstoy", BirthDate = DateTime.Parse("1860-9-21"), Nationality = "Russian", Gender = "Male" },
                    new Author { /*Id = 2, */FirstName = "Albert", LastName = "Camus", BirthDate = DateTime.Parse("1831-11-19"), Nationality = "French", Gender = "Male" },
                    new Author { /*Id = 3, */FirstName = "Sylvia", LastName = "Plath", BirthDate = DateTime.Parse("1886-10-21"), Nationality = "American", Gender = "Female" },
                    new Author { /*Id = 4, */FirstName = "Fyodor", LastName = "Dostoevsky", BirthDate = DateTime.Parse("1850-9-21"), Nationality = "Russian", Gender = "Male" },
                    new Author { /*Id = 5, */FirstName = "Mary", LastName = "Shelley", BirthDate = DateTime.Parse("1852-7-1"), Nationality = "British", Gender = "Female" },
                    new Author { /*Id = 6, */FirstName = "Franz", LastName = "Kafka", BirthDate = DateTime.Parse("1903-11-8"), Nationality = "Austrian", Gender = "Male" },
                    new Author { /*Id = 7, */FirstName = "Ivo", LastName = "Andrić", BirthDate = DateTime.Parse("1907-5-26"), Nationality = "Serbian", Gender = "Male" },
                    new Author { /*Id = 8, */FirstName = "John", LastName = "Tolkien", BirthDate = DateTime.Parse("1817-6-7"), Nationality = "British", Gender = "Male"  },
                    new Author { /*Id = 9, */FirstName = "Joanne", LastName = "Rowling", BirthDate = DateTime.Parse("1965-6-7"), Nationality = "British", Gender = "Female" }
                );
                context.SaveChanges();

                context.Book.AddRange(
                    new Book
                    {
                        //Id = 1,
                        Title = "War and Peace",
                        YearPublished = 1899,
                        NumPages = 1225,
                        Description = "Leo Tolstoy's \"War and Peace\" is a novel that depicts the lives of Russian aristocrats during the Napoleonic Wars, exploring themes such as love, family, war, and the search for meaning in life.",
                        Publisher = "The Russian Messanger",
                        DownloadUrl = "WarAndPeace.pdf",
                        FrontPage = "WarAndPeace.jpg",
                        AuthorId = 1
                    },
                    new Book{
                    //Id = 2,
                    Title = "The Castle",
                        YearPublished = 1926,
                        NumPages = 820,
                        Description = "\"The Castle\" by Franz Kafka is a novel that follows a land surveyor named K. who is summoned to a village by authorities who work in a mysterious castle. Despite his efforts, K. struggles to gain access to the castle and faces obstacles from the villagers who seem to be controlled by the castle's unseen authority. The novel explores themes of bureaucracy, alienation, and the search for meaning in a seemingly incomprehensible world.",
                        Publisher = "Kurt Wolff",
                        DownloadUrl = "TheCastle.pdf",
                        FrontPage = "TheCastle.jpg",
                        AuthorId = 6
                    },
                    new Book
                    {
                        //Id = 3,
                        Title = "The Trial",
                        YearPublished = 1930,
                        NumPages = 320,
                        Description = "\"The Trial\" is a novel by Franz Kafka about a man named Josef K. who is arrested and put on trial for a crime he is never informed of, and the surreal and absurd experiences he has as he tries to understand and navigate the mysterious and oppressive legal system.",
                        Publisher = "Kurt Wolff",
                        DownloadUrl = "TheTrial.pdf",
                        FrontPage = "TrialKafka.jpg",
                        AuthorId = 6
                    },
                    new Book
                    {
                        //Id = 4,
                        Title = "Frankenstein",
                        YearPublished = 1860,
                        NumPages = 280,
                        Description = "\"Frankenstein\" is a gothic novel written by Mary Shelley in 1818, which tells the story of a young scientist named Victor Frankenstein who creates a monstrous creature in his laboratory, but soon realizes that his creation may be more dangerous than he ever imagined.",
                        Publisher = "Lackington",
                        DownloadUrl = "Frankenstein.pdf",
                        FrontPage = "Frankenstein.jpg",
                        AuthorId = 5
                    },
                    new Book
                    {
                        //Id = 5,
                        Title = "The Lord of the Rings",
                        YearPublished = 1860,
                        NumPages = 1280,
                        Description = "\"The Lord of the Rings\" is a high-fantasy novel by J.R.R. Tolkien that follows hobbit Frodo Baggins as he embarks on a perilous journey to destroy a powerful ring that could bring about the end of Middle-earth, while facing various dangers and encountering memorable characters along the way.",
                        Publisher = "Allen & Unwin",
                        DownloadUrl = "The-lord-of-the-rings.pdf",
                        FrontPage = "LordOfTheRings.jpg",
                        AuthorId = 8
                    },
                    new Book
                    {
                        //Id = 6,
                        Title = "Harry Potter and the Philosopher's Stone",
                        YearPublished = 1997,
                        NumPages = 223,
                        Description = "\"Harry Potter and the Philosopher's Stone\" is a fantasy novel written by J.K. Rowling. It follows the story of an orphaned boy named Harry Potter who discovers he is a wizard and is invited to attend Hogwarts School of Witchcraft and Wizardry. Along with his new friends Ron and Hermione, Harry sets out to unravel the mystery of the Philosopher's Stone, a powerful object that could grant immortality to its owner, while also facing the threat of the evil wizard, Lord Voldemort.",
                        Publisher = "Bloomsbury",
                        DownloadUrl = "Harry Potter and the Sorcerers Stone.pdf",
                        FrontPage = "Harry_Potter_and_the_Philosopher's_Stone.jpg",
                        AuthorId = 9
                    },
                    new Book
                    {
                        //Id = 7,
                        Title = "The Bridge on the Drina",
                        YearPublished = 1945,
                        NumPages = 318,
                        Description = "\"The Bridge on the Drina\" is a historical fiction novel written by Yugoslav author Ivo Andrić. It chronicles the history of the town of Višegrad and its Ottoman-era bridge over the Drina River, examining the impact of war, politics, and social change on the community and the bridge itself over the course of four centuries.",
                        Publisher = "Prosveta",
                        DownloadUrl = "The-Bridge-On-The-Drina.pdf",
                        FrontPage = "TheBridgeInTheDrine.jpg",
                        AuthorId = 7
                    },
                    new Book
                    {
                        //Id = 8,
                        Title = "Crime and Punishment",
                        YearPublished = 1867,
                        NumPages = 550,
                        Description = "\"Crime and Punishment\" is a psychological novel written by Fyodor Dostoevsky, first published in 1866. It tells the story of a young impoverished ex-student, Rodion Raskolnikov, who plans and carries out the murder of a pawnbroker for her money. The novel explores themes of guilt, redemption, and the consequences of moral transgressions",
                        Publisher = "The Russian Messanger",
                        DownloadUrl = "CrimeAndPunishment.pdf",
                        FrontPage = "CrimeAndPunishment.jpg",
                        AuthorId = 4
                    },
                    new Book
                    {
                        //Id = 9,
                        Title = "The Brothers Karamazov",
                        YearPublished = 1890,
                        NumPages = 1221,
                        Description = "\"The Brothers Karamazov\" is a novel by Russian author Fyodor Dostoevsky, published in 1880. It tells the story of the Karamazov family and their dysfunctional relationships, particularly the three brothers Dmitry, Ivan, and Alexei. The novel explores themes of religion, morality, and human nature, and is considered one of the greatest works of world literature.",
                        Publisher = "The Russian Messanger",
                        DownloadUrl = "TheBrothersKaramazov.pdf",
                        FrontPage = "TheBrothersKaramazov.jpg",
                        AuthorId = 4
                    },
                    new Book
                    {
                        //Id = 10,
                        Title = "The Bell Jar",
                        YearPublished = 1963,
                        NumPages = 244,
                        Description = "\"The Bell Jar\" is a semi-autobiographical novel by Sylvia Plath that tells the story of a young woman named Esther Greenwood as she struggles with mental illness and societal expectations in the 1950s.",
                        Publisher = "Heinemann",
                        DownloadUrl = "TheBellJar.pdf",
                        FrontPage = "TheBellJar.jpg",
                        AuthorId = 3
                    },
                    new Book
                    {
                        //Id = 11,
                        Title = "The Stranger",
                        YearPublished = 1942,
                        NumPages = 160,
                        Description = "\"The Stranger\" is a philosophical novel written by Albert Camus in which the protagonist, Meursault, navigates life with a sense of detachment and indifference, culminating in a murder trial that challenges societal norms and beliefs.",
                        Publisher = "Gallimard",
                        DownloadUrl = "TheStranger.pdf",
                        FrontPage = "TheStranger.jpg",
                        AuthorId = 2
                    }
                );

                context.SaveChanges();

                context.Genre.AddRange(
                    new Genre
                    {
                        // Id = 1
                        GenreName = "Novel"
                    },
                    new Genre
                    {
                        // Id = 2
                        GenreName = "Autobiography"
                    },
                    new Genre
                    {
                        // Id = 3
                        GenreName = "Psychological Fiction"
                    },
                    new Genre
                    {
                        // Id = 4
                        GenreName = "Crime Fiction"
                    },
                    new Genre
                    {
                        // Id = 5
                        GenreName = "Comedy"
                    },
                    new Genre
                    {
                        // Id = 6
                        GenreName = "Dystopian Fiction"
                    },
                    new Genre
                    {
                        // Id = 7
                        GenreName = "Gothic Fiction"
                    },
                    new Genre
                    {
                        // Id = 8
                        GenreName = "Science Fiction"
                    },
                    new Genre
                    {
                        // Id = 9
                        GenreName = "Horror Fiction"
                    },
                    new Genre
                    {
                        // Id = 10
                        GenreName = "Historical Fiction"
                    },
                    new Genre
                    {
                        // Id = 11
                        GenreName = "Romance Novel"
                    },
                    new Genre
                    {
                        // Id = 12
                        GenreName = "Fantasy Fiction"
                    },
                    new Genre
                    {
                        // Id = 13
                        GenreName = "Fairy Tale"
                    },
                    new Genre
                    {
                        // Id = 14
                        GenreName = "Advanture Tale"
                    },
                    new Genre
                    {
                        // Id = 15
                        GenreName = "Philosophical fiction"
                    }
                );

                context.SaveChanges();

                context.bookGenres.AddRange(
                    new BookGenre
                    {
                        BookId = 1,
                        GenreId = 1
                    },
                    new BookGenre
                    {
                        BookId = 1,
                        GenreId = 10
                    },
                    new BookGenre
                    {
                        BookId = 1,
                        GenreId = 15
                    },
                    new BookGenre
                    {
                        BookId = 2,
                        GenreId = 1
                    },
                    new BookGenre
                    {
                        BookId = 2,
                        GenreId = 5
                    },
                    new BookGenre
                    {
                        BookId = 2,
                        GenreId = 15
                    },
                    new BookGenre
                    {
                        BookId = 3,
                        GenreId = 1
                    },
                    new BookGenre
                    {
                        BookId = 3,
                        GenreId = 15
                    },
                    new BookGenre
                    {
                        BookId = 3,
                        GenreId = 6
                    },
                    new BookGenre
                    {
                        BookId = 4,
                        GenreId = 7
                    },
                    new BookGenre
                    {
                        BookId = 4,
                        GenreId = 8
                    },
                    new BookGenre
                    {
                        BookId = 4,
                        GenreId = 9
                    },
                    new BookGenre
                    {
                        BookId = 5,
                        GenreId = 12
                    },
                    new BookGenre
                    {
                        BookId = 5,
                        GenreId = 13
                    },
                    new BookGenre
                    {
                        BookId = 5,
                        GenreId = 14
                    },
                    new BookGenre
                    {
                        BookId = 6,
                        GenreId = 12
                    },
                    new BookGenre
                    {
                        BookId = 6,
                        GenreId = 13
                    },
                    new BookGenre
                    {
                        BookId = 7,
                        GenreId = 1
                    },
                    new BookGenre
                    {
                        BookId = 7,
                        GenreId = 10
                    },
                    new BookGenre
                    {
                        BookId = 8,
                        GenreId = 1
                    },
                    new BookGenre
                    {
                        BookId = 8,
                        GenreId = 10
                    },
                    new BookGenre
                    {
                        BookId = 8,
                        GenreId = 15
                    },
                    new BookGenre
                    {
                        BookId = 9,
                        GenreId = 1
                    },
                    new BookGenre
                    {
                        BookId = 9,
                        GenreId = 10
                    },
                    new BookGenre
                    {
                        BookId = 10,
                        GenreId = 1
                    },
                    new BookGenre
                    {
                        BookId = 10,
                        GenreId = 2
                    },
                    new BookGenre
                    {
                        BookId = 10,
                        GenreId = 3
                    },
                    new BookGenre
                    {
                        BookId = 11,
                        GenreId = 1
                    },
                    new BookGenre
                    {
                        BookId = 11,
                        GenreId = 4
                    },
                    new BookGenre
                    {
                        BookId = 11,
                        GenreId = 15
                    }
                );

                context.SaveChanges();

                context.Review.AddRange(
                    new Review
                    {
                        BookId = 1,
                        AppUser = "Bob",
                        Rating = 9,
                        Comment = "The descriptions in this book were so vivid that I felt like I was right there in the story."
                    },
                    new Review
                    {
                        BookId = 1,
                        AppUser = "Alice",
                        Rating = 8,
                        Comment = "The writing style was beautiful and the author really captured the essence"
                    },
                    new Review
                    {
                        BookId = 1,
                        AppUser = "John",
                        Rating = 7,
                        Comment = "I found the characters to be relatable and their struggles felt very real"
                    },
                    new Review
                    {
                        BookId = 2,
                        AppUser = "John",
                        Rating = 10,
                        Comment = "This book was a real emotional rollercoaster. I laughed, I cried, and I couldn't put it down."
                    },
                    new Review
                    {
                        BookId = 2,
                        AppUser = "Stefan",
                        Rating = 9,
                        Comment = "The author's writing style was engaging and kept me hooked from beginning to end"
                    },
                    new Review
                    {
                        BookId = 2,
                        AppUser = "Trey",
                        Rating = 9,
                        Comment = "The world-building in this book was fantastic and really drew me in"
                    },
                    new Review
                    {
                        BookId = 3,
                        AppUser = "Alice",
                        Rating = 8,
                        Comment = "While the ending wasn't quite what I was expecting, it was still satisfying and tied up all the loose ends"
                    },
                    new Review
                    {
                        BookId = 3,
                        AppUser = "Anne",
                        Rating = 7,
                        Comment = "I really enjoyed the unique perspective the author brought"
                    },
                    new Review
                    {
                        BookId = 4,
                        AppUser = "Bob",
                        Rating = 5,
                        Comment = "I didn't like the way this book ended. It felt rushed and unsatisfying"
                    },
                    new Review
                    {
                        BookId = 4,
                        AppUser = "Trey",
                        Rating = 7,
                        Comment = "I would highly recommend this book to anyone."
                    },
                    new Review
                    {
                        BookId = 4,
                        AppUser = "Stefan",
                        Rating = 4,
                        Comment = "This book was a disappointment. The plot was predictable and the characters felt flat."
                    },
                    new Review
                    {
                        BookId = 5,
                        AppUser = "Bob",
                        Rating = 9,
                        Comment = "The author's writing style was engaging and kept me hooked from beginning to end."
                    },
                    new Review
                    {
                        BookId = 5,
                        AppUser = "Alice",
                        Rating = 4,
                        Comment = "This book was a disappointment. The plot was predictable and the characters felt flat."
                    },
                    new Review
                    {
                        BookId = 6,
                        AppUser = "Trey",
                        Rating = 7,
                        Comment = "I found the characters to be relatable and their struggles felt very real."
                    },
                    new Review
                    {
                        BookId = 6,
                        AppUser = "Penny",
                        Rating = 8,
                        Comment = "The writing style was beautiful and the author really captured the essence."
                    },
                    new Review
                    {
                        BookId = 7,
                        AppUser = "Anne",
                        Rating = 9,
                        Comment = "The author's writing style was engaging and kept me hooked from beginning to end."
                    },
                    new Review
                    {
                        BookId = 7,
                        AppUser = "Penny",
                        Rating = 7,
                        Comment = "The world-building in this book was fantastic and really drew me in."
                    },
                    new Review
                    {
                        BookId = 8,
                        AppUser = "Emily",
                        Rating = 10,
                        Comment = "Overall, this was a fantastic read and I can't wait to see what this author comes up with next."
                    },
                    new Review
                    {
                        BookId = 8,
                        AppUser = "Stefan",
                        Rating = 10,
                        Comment = "I had a hard time getting into this book at first, but once I did, I really enjoyed it."
                    },
                    new Review
                    {
                        BookId = 9,
                        AppUser = "Alice",
                        Rating = 10,
                        Comment = "I didn't like the way this book ended. It felt rushed and unsatisfying."
                    },
                    new Review
                    {
                        BookId = 9,
                        AppUser = "Bob",
                        Rating = 8,
                        Comment = "The themes explored in this book were important and thought-provoking."
                    },
                    new Review
                    {
                        BookId = 10,
                        AppUser = "Stefan",
                        Rating = 8,
                        Comment = "This book was a real page-turner. It kept me guessing until the very end."
                    },
                    new Review
                    {
                        BookId = 11,
                        AppUser = "Emily",
                        Rating = 10,
                        Comment = "The pacing was perfect and the story flowed seamlessly from beginning to end."
                    }
                );

                context.SaveChanges();
            }
        }
    }
}
