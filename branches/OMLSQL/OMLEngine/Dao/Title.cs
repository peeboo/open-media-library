using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OMLEngine.Dao
{
    public enum PeopleRoles : byte
    {
        Actor = 0,
        Director = 1,
        Writer = 2,
        Producers = 3,
        NonActing = 4 // this is a placeholder - we should flush this out to include all film roles
    }

    internal partial class Title
    {
        /// <summary>
        /// Will create an oml engine title from a dao title.
        /// // todo : solomon : this should go away when the title objects are combined
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static OMLEngine.Title CreateOMLEngineTitleFromTitle(Title title, bool includePeople)
        {
            if (title == null)
                return null;

            OMLEngine.Title returnTitle = new OMLEngine.Title(title.Id);

            returnTitle.AspectRatio = title.AspectRatio;
            returnTitle.CountryOfOrigin = title.CountryOfOrgin;
            returnTitle.DateAdded = (DateTime)title.DateAdded;
            returnTitle.Name = title.Name;
            returnTitle.ParentalRating = title.ParentalRating ?? string.Empty;
            returnTitle.ParentalRatingReason = title.ParentalRatingReason;
            returnTitle.ReleaseDate = (DateTime)title.ReleaseDate;
            returnTitle.Runtime = (int)title.Runtime;
            returnTitle.SortName = title.SortName;
            returnTitle.Studio = title.Studio;
            //doaTitle.Subtitles = title.Subtitles; // todo : solomon : figure out how to store this
            returnTitle.Synopsis = title.Synopsis;
            returnTitle.UPC = title.UPC;
            returnTitle.UserStarRating = (int)title.UserRating;
            returnTitle.VideoDetails = title.VideoDetails;
            returnTitle.VideoResolution = title.VideoResolution;
            returnTitle.VideoStandard = title.VideoStandard;
            returnTitle.WatchedCount = title.WatchedCount ?? 0;
            returnTitle.OfficialWebsiteURL = title.WebsiteUrl;

            returnTitle.FrontCoverPath = title.FrontCoverImagePath;
            returnTitle.FrontCoverMenuPath = title.MenuImagePath;
            returnTitle.BackCoverPath = title.BackCoverImagePath;

            // add the genres
            foreach (Genre genre in title.Genres)
            {
                returnTitle.Genres.Add(genre.MetaData.Name);
            }

            // add the tags
            foreach (Tag tag in title.Tags)
            {
                returnTitle.Tags.Add(tag.Name);
            }

            // add the people
            if (includePeople)
            {
                foreach (Person person in title.People)
                {
                    switch ((PeopleRoles) person.Role)
                    {
                        case PeopleRoles.Actor:
                            returnTitle.ActingRoles.Add(person.MetaData.FullName, person.CharacterName);
                            break;

                        case PeopleRoles.Director:
                            returnTitle.Directors.Add(new OMLEngine.Person(person.MetaData.FullName));
                            break;

                        case PeopleRoles.Producers:
                            returnTitle.Producers.Add(person.MetaData.FullName);
                            break;

                        case PeopleRoles.Writer:
                            returnTitle.Writers.Add(new OMLEngine.Person(person.MetaData.FullName));
                            break;
                    }
                }
            }

            // add the disks
            foreach (Disk disk in title.Disks)
            {
                returnTitle.Disks.Add(new OMLEngine.Disk(disk.Name, disk.Path, (VideoFormat)disk.VideoFormat));
            }

            return returnTitle;
        }

        /// <summary>
        /// Will create a title object from an engine title
        /// todo : solomon : this should go away and engine title should just contain the dao title object
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static Title CreateFromOMLEngineTitle(OMLEngine.Title title)
        {
            Title daoTitle = new Title();

            daoTitle.AspectRatio = title.AspectRatio;
            daoTitle.CountryOfOrgin = title.CountryOfOrigin;
            daoTitle.DateAdded = title.DateAdded;
            daoTitle.Name = title.Name;
            daoTitle.ParentalRating = title.ParentalRating;
            daoTitle.ParentalRatingReason = title.ParentalRatingReason;
            daoTitle.ReleaseDate = title.ReleaseDate;
            daoTitle.Runtime = (short) title.Runtime;
            daoTitle.SortName = title.SortName;
            daoTitle.Studio = title.Studio;
            //doaTitle.Subtitles = title.Subtitles; // todo : solomon : figure out how to store this
            daoTitle.Synopsis = title.Synopsis;
            daoTitle.UPC = title.UPC;
            daoTitle.UserRating = (byte) title.UserStarRating;
            daoTitle.VideoDetails = title.VideoDetails;
            daoTitle.VideoResolution = title.VideoResolution;
            daoTitle.VideoStandard = title.VideoStandard;
            daoTitle.WatchedCount = title.WatchedCount;
            daoTitle.WebsiteUrl = title.OfficialWebsiteURL;

            daoTitle.FrontCoverImagePath = title.FrontCoverPath;
            daoTitle.MenuImagePath = title.FrontCoverMenuPath;
            daoTitle.BackCoverImagePath = title.BackCoverPath;

            // add the genres
            foreach (string genre in title.Genres)
            {
                // see if we've added this genre locally already
                Genre daoGenre = daoTitle.Genres.FirstOrDefault(t => t.MetaData.Name.Equals(genre));

                // genres must be unique
                if (daoGenre != null)
                    continue;

                // try to see if the genre exists
                GenreMetaData metaData = TitleCollectionDao.GetGenreMetaDataByName(genre);

                if (metaData == null)
                {
                    // if it doesn't exist create a new one                    
                    metaData = new GenreMetaData();
                    metaData.Name = genre;
                }

                // setup the genre
                daoGenre = new Genre();
                daoGenre.MetaData = metaData;

                // add the genre to the title
                daoTitle.Genres.Add(daoGenre);
            }

            // add the tags
            foreach (string name in title.Tags)
            {
                // see if we've added this tag locally already
                Tag tag = daoTitle.Tags.FirstOrDefault(t => t.Name.Equals(name));

                // tags must be unique
                if (tag != null)
                    continue;

                // try to see if the tag exists in the db already
                tag = TitleCollectionDao.GetTagByTagName(name);

                if (tag == null)
                {
                    // if it doesn't exist create a new one
                    tag = new Tag();
                    tag.Name = name;
                }

                // add the tag
                daoTitle.Tags.Add(tag);
            }

            // grab from the db who we know about already
            Dictionary<string, BioData> existingPeople = GetAllExistingPeople(title);

            int actorIndex = 0;            
            
            // add the actors
            foreach (string actor in title.ActingRoles.Keys)
            {
                Person person = CreatePerson(actor, title.ActingRoles[actor], PeopleRoles.Actor, existingPeople);

                // maintain the order
                person.Sort = (short)(actorIndex++);

                // add them to the title
                daoTitle.People.Add(person);               
            }            

            // add the directors
            foreach (OMLEngine.Person director in title.Directors)
            {
                Person person = CreatePerson(director.full_name, null, PeopleRoles.Director, existingPeople);

                // maintain the order
                person.Sort = (short)(actorIndex++);

                // add them to the title
                daoTitle.People.Add(person);
            }

            // add the writers
            foreach (OMLEngine.Person writer in title.Writers)
            {
                Person person = CreatePerson(writer.full_name, null, PeopleRoles.Writer, existingPeople);

                // maintain the order
                person.Sort = (short)(actorIndex++);

                // add them to the title
                daoTitle.People.Add(person);
            }

            // add the producers
            foreach (string name in title.Producers)
            {
                Person person = CreatePerson(name, null, PeopleRoles.Producers, existingPeople);

                // maintain the order
                person.Sort = (short)(actorIndex++);

                // add them to the title
                daoTitle.People.Add(person);
            }

            // ignore the rest for now 

            // add all the disks
            foreach( OMLEngine.Disk disk in title.Disks)
            {
                Disk daoDisk = new Disk();
                daoDisk.Name = disk.Name;
                daoDisk.Path = disk.Path;
                daoDisk.VideoFormat = (byte)disk.Format;

                daoTitle.Disks.Add(daoDisk);
            }

            return daoTitle;
        }

        /// <summary>
        /// Go through all the people in the movie and send them in batch to sql to see who exists
        /// as a person already
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        private static Dictionary<string, BioData> GetAllExistingPeople(OMLEngine.Title title)
        {
            List<string> names = new List<string>(title.ActingRoles.Count + title.Writers.Count + title.Directors.Count + title.Producers.Count);

            foreach (string person in title.ActingRoles.Keys)
                names.Add(person);

            foreach (OMLEngine.Person person in title.Writers)
                names.Add(person.full_name);

            foreach (OMLEngine.Person person in title.Directors)
                names.Add(person.full_name);

            foreach (string person in title.Producers)
                names.Add(person);

            // now that we have all the people - query to see who exists
            var actors = from actor in DBContext.Instance.BioDatas
                         where names.Contains(actor.FullName)
                         select actor;

            Dictionary<string, BioData> existingPeople = new Dictionary<string, BioData>(actors.Count());

            foreach (BioData data in actors)
                if (!existingPeople.ContainsKey(data.FullName))
                    existingPeople.Add(data.FullName, data);

            return existingPeople;
        }

        /// <summary>
        /// Creates a person object from a given name and role.  Will check the db first to see if that person exists
        /// </summary>
        /// <param name="name"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        private static Person CreatePerson(string name, string characterName, PeopleRoles role, Dictionary<string, BioData> existingPeople)
        {            
            BioData metaData = null;

            // see if the actor exists already
            existingPeople.TryGetValue(name, out metaData);

            if (metaData == null)
            {
                // if it doesn't exist create a new one
                metaData = new BioData();
                metaData.FullName = name;

                // add the new metaData we added to the dictionary so we don't add it again
                existingPeople.Add(name, metaData);
            }

            // setup the person
            Person person = new Person();
            person.MetaData = metaData;
            person.Role = (byte)role;

            if (!string.IsNullOrEmpty(characterName))
            {
                person.CharacterName = characterName;
            }

            return person;
        }
    }
}
