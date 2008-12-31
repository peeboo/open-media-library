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
            //doaTitle.ParentalRating = title.ParentalRating;
            daoTitle.ParentalRatingReason = title.ParentalRatingReason;
            daoTitle.ReleaseDate = title.ReleaseDate;
            daoTitle.Runtime = (short) title.Runtime;
            daoTitle.SortName = title.SortName;
            daoTitle.Studio = title.Studio;
            //doaTitle.Subtitles = title.Subtitles;
            daoTitle.Synopsis = title.Synopsis;
            daoTitle.UPC = title.UPC;
            daoTitle.UserRating = (byte) title.UserStarRating;
            daoTitle.VideoDetails = title.VideoDetails;
            daoTitle.VideoResolution = title.VideoResolution;
            daoTitle.VideoStandard = title.VideoStandard;
            daoTitle.WatchedCount = title.WatchedCount;
            daoTitle.WebsiteUrl = title.OfficialWebsiteURL;

            // add the genres
            foreach (string genre in title.Genres)
            {               
                // try to see if the genre exists
                GenreMetaData metaData = TitleCollectionDao.GetGenreMetaDataByName(genre);
                
                if (metaData == null)
                {                
                    // if it doesn't create a new one                    
                    metaData = new GenreMetaData();
                    metaData.Name = genre;
                }
                
                // setup the genre
                Genre daoGenre = new Genre();
                daoGenre.MetaData = metaData;                

                // add the genre to the title
                daoTitle.Genres.Add(daoGenre);
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
            // see if the actor exists already
            BioData metaData = existingPeople.ContainsKey(name) ? existingPeople[name] : null; //TitleCollectionDao.GetPersonBioDataByName(name);

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
