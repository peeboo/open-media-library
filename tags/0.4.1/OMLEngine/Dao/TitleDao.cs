using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OMLEngine.Dao
{    
    internal static class TitleDao
    {
        private const char dbCollectionDelimiter = '\t';        

        public static void SetupCollectionsToBeAdded(OMLDataDataContext context, OMLEngine.Title title)
        {
            Title daoTitle = title.DaoTitle;

            // patch up the sort name if it's missing
            if (string.IsNullOrEmpty(daoTitle.SortName))
                daoTitle.SortName = daoTitle.Name;

            // add the genres
            foreach (string genre in title.Genres)
            {
                // see if we've added this genre locally already
                Genre daoGenre = daoTitle.Genres.FirstOrDefault(t => t.MetaData.Name.Equals(genre));

                // genres must be unique
                if (daoGenre != null)
                    continue;

                // try to see if the genre exists
                GenreMetaData metaData = context.GenreMetaDatas.SingleOrDefault(t => t.Name.ToLower() == genre.ToLower());

                if (metaData == null)
                {
                    // if it doesn't exist create a new one                    
                    metaData = new GenreMetaData();
                    metaData.Name = genre;
                    context.GenreMetaDatas.InsertOnSubmit(metaData);
                    context.SubmitChanges();
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
                tag = context.Tags.SingleOrDefault(t => t.Name.ToLower() == name.ToLower());

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
            Dictionary<string, BioData> existingPeople = GetAllExistingPeople(context, title);

            int actorIndex = 0;

            // add the actors
            foreach (Role actor in title.DaoTitle.UpdatedActors)
            {
                Person person = CreatePerson(context, actor.PersonName, actor.RoleName, PeopleRole.Actor, existingPeople);

                // maintain the order
                person.Sort = (short)(actorIndex++);

                // add them to the title
                daoTitle.People.Add(person);
            }

            // add the directors
            foreach (OMLEngine.Person director in title.DaoTitle.UpdatedDirectors)
            {
                Person person = CreatePerson(context, director.full_name, null, PeopleRole.Director, existingPeople);

                // maintain the order
                person.Sort = (short)(actorIndex++);

                // add them to the title
                var e = (from p in daoTitle.People
                         where p.MetaData.Id == person.MetaData.Id
                         select p);

                if (e.Count() == 0)
                {
                    daoTitle.People.Add(person);
                }
            }

            // add the writers
            foreach (OMLEngine.Person writer in title.DaoTitle.UpdatedWriters)
            {
                Person person = CreatePerson(context, writer.full_name, null, PeopleRole.Writer, existingPeople);

                // maintain the order
                person.Sort = (short)(actorIndex++);

                // add them to the title
                daoTitle.People.Add(person);
            }

            // add the producers
            foreach (OMLEngine.Person name in title.DaoTitle.UpdatedProducers)
            {
                Person person = CreatePerson(context, name.full_name, null, PeopleRole.Producers, existingPeople);

                // maintain the order
                person.Sort = (short)(actorIndex++);

                // add them to the title
                daoTitle.People.Add(person);
            }

            // Debugging code
            var b = (from p in daoTitle.People
                     select p);
            foreach (Person pr in b)
            {
                System.Diagnostics.Trace.WriteLine("Adding " + pr.Role + " - " + pr.MetaData.FullName + " as " + pr.CharacterName + " [" + pr.MetaData.Id +"]");
            }



            // ignore the rest for now 

            // add all the disks
            /*foreach (OMLEngine.Disk disk in title.Disks)
            {
                Disk daoDisk = new Disk();
                daoDisk.Name = disk.Name;
                daoDisk.Path = disk.Path;
                daoDisk.VideoFormat = (byte)disk.Format;

                daoTitle.Disks.Add(daoDisk);
            }

            // add the audio tracks            
            daoTitle.AudioTracks = GetDelimitedStringFromCollection(title.AudioTracks);

            // add the subtitles            
            daoTitle.Subtitles = GetDelimitedStringFromCollection(title.Subtitles);

            // add the trailers             
            daoTitle.Trailers = GetDelimitedStringFromCollection(title.Trailers);*/
        }

        public static void UpdateCollectionsForTitle(Dao.Title title)
        {
            if (title.UpdatedActors != null)
            {
                IEnumerable<string> originalActors = from a in title.People
                                                     where a.Role == (byte)PeopleRole.Actor
                                                     select a.MetaData.FullName;

                List<string> added = new List<string>(title.UpdatedActors.Where(t => !originalActors.Contains(t.PersonName)).Select(t => t.PersonName));
                List<string> removed = new List<string>(originalActors.Where(t => !title.UpdatedActors.Select(r => r.PersonName).Contains(t)));

                // remove ones no longer used
                foreach (string remove in removed)
                {
                    Dao.Person person = title.People.SingleOrDefault(p => p.MetaData.FullName == remove && p.Role == (byte)PeopleRole.Actor);

                    if (person != null)
                        title.People.Remove(person);
                }

                Dictionary<string, string> actorLookup = new Dictionary<string, string>();
                title.UpdatedActors.ForEach(t => actorLookup.Add(t.PersonName, t.RoleName));

                // add the new ones
                foreach (string add in added)
                {
                    AddActorToTitle(title, add, actorLookup[add], PeopleRole.Actor);                    
                }
            }

            if (title.UpdatedDirectors != null)
            {
                ProcessPersonList(title, title.UpdatedDirectors, PeopleRole.Director);                
            }

            if (title.UpdatedWriters != null)
            {
                ProcessPersonList(title, title.UpdatedWriters, PeopleRole.Writer);
            }

            if (title.UpdatedProducers != null)
            {
                ProcessPersonList(title, title.UpdatedProducers, PeopleRole.Producers);
            }

            // if the genres were modified see how they've changed
            if (title.UpdatedGenres != null)
            {
                // see if there are any genres to add

                // grab all the original genres
                IEnumerable<string> originalGenres = from g in title.Genres
                                                     select g.MetaData.Name;

                List<string> added = new List<string>(title.UpdatedGenres.Where(t => !originalGenres.Contains(t)));
                List<string> removed = new List<string>(originalGenres.Where(t => !title.UpdatedGenres.Contains(t)));

                // remove ones no longer used
                foreach (string remove in removed)
                {
                    Dao.Genre genre = title.Genres.SingleOrDefault(g => g.MetaData.Name == remove);

                    if (genre != null)
                        title.Genres.Remove(genre);
                }

                // add the new ones
                foreach (string add in added)
                {
                    AddGenreToTitle(title, add);
                }
            }

            if (title.UpdatedTags != null)
            {
                IEnumerable<string> originalTags = from t in title.Tags
                                                   select t.Name;

                List<string> added = new List<string>(title.UpdatedTags.Where(t => !originalTags.Contains(t)));
                List<string> removed = new List<string>(originalTags.Where(t => !title.UpdatedTags.Contains(t)));

                foreach (string remove in removed)
                {
                    Dao.Tag tag = title.Tags.SingleOrDefault(t => t.Name == remove);

                    if (tag != null)
                        title.Tags.Remove(tag);
                }

                foreach (string add in added)
                {
                    Dao.Tag daoTag = new OMLEngine.Dao.Tag();
                    daoTag.TitleId = title.Id;
                    daoTag.Name = add;

                    title.Tags.Add(daoTag);
                }
            }
        }

        private static void ProcessPersonList(Dao.Title title, List<OMLEngine.Person> updatedList, PeopleRole role)
        {
            IEnumerable<string> originals = from a in title.People
                                                  where a.Role == (byte)role
                                                  select a.MetaData.FullName;

            List<string> added = new List<string>(updatedList.Where(t => !originals.Contains(t.full_name)).Select(t => t.full_name));
            List<string> removed = new List<string>(originals.Where(t => !updatedList.Select(r => r.full_name).Contains(t)));

            // remove ones no longer used
            foreach (string remove in removed)
            {
                Dao.Person person = title.People.SingleOrDefault(p => p.MetaData.FullName == remove && p.Role == (byte)role);

                if (person != null)
                    title.People.Remove(person);
            }

            // add the new ones
            foreach (string add in added)
            {
                AddActorToTitle(title, add, null, role);
            }
        }

        private static void AddActorToTitle(Dao.Title title, string actor, string role, PeopleRole type)
        {
            if (actor.Length > 255)
                throw new FormatException("Actor must be 255 characters or less.");
            if (role != null && role.Length > 255)
                throw new FormatException("Role must be 255 characters or less.");

            if (string.IsNullOrEmpty(actor))
                return;

            Dao.BioData bioData = Dao.TitleCollectionDao.GetPersonBioDataByName(actor);

            if (bioData == null)
            {
                bioData = new OMLEngine.Dao.BioData();
                bioData.FullName = actor;
                Dao.DBContext.Instance.BioDatas.InsertOnSubmit(bioData);
                Dao.DBContext.Instance.SubmitChanges();
            }

            Dao.Person person = new OMLEngine.Dao.Person();
            person.MetaData = bioData;
            person.CharacterName = role;
            person.Role = (byte)type;
            title.People.Add(person);
        }        

        /// <summary>
        /// Adds a genre to a title
        /// </summary>
        /// <param name="title"></param>
        /// <param name="genre"></param>
        private static void AddGenreToTitle(Dao.Title title, string genre)
        {
            if (string.IsNullOrEmpty(genre))
                return;

            // see if the genre exists
            Dao.GenreMetaData meta = Dao.TitleCollectionDao.GetGenreMetaDataByName(genre);

            if (meta == null)
            {
                meta = new OMLEngine.Dao.GenreMetaData();
                meta.Name = genre;

                // save the genre
                Dao.DBContext.Instance.GenreMetaDatas.InsertOnSubmit(meta);
            }

            title.Genres.Add(new Dao.Genre { MetaData = meta });
        }

        /// <summary>
        /// Turns a collection into a delimted string
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static string GetDelimitedStringFromCollection(IList<string> collection)
        {
            if (collection == null || collection.Count == 0)
                return null;

            StringBuilder delimitedString = new StringBuilder(collection.Count * 30);
            for (int x = 0; x < collection.Count; x++)
            {
                delimitedString.Append(collection[x]);

                if (x != collection.Count - 1)
                    delimitedString.Append(dbCollectionDelimiter);
            }

            return delimitedString.ToString();
        }       

        /// <summary>
        /// Given a DB delimited string return a collection
        /// </summary>
        /// <param name="delimitedString"></param>
        /// <returns></returns>
        public static List<string> DelimitedDBStringToCollection(string delimitedString)
        {
            if (delimitedString == null)
                return new List<string>(0);

            string[] parts = delimitedString.Split(dbCollectionDelimiter);

            List<string> returnParts = new List<string>(parts.Length);

            foreach (string part in parts)
                if (!string.IsNullOrEmpty(part))
                    returnParts.Add(part);

            return returnParts;
        }

        /// <summary>
        /// Go through all the people in the movie and send them in batch to sql to see who exists
        /// as a person already
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        private static Dictionary<string, BioData> GetAllExistingPeople(OMLDataDataContext context, OMLEngine.Title title)
        {
            List<string> names = new List<string>(title.ActingRoles.Count + title.Writers.Count + title.Directors.Count + title.Producers.Count);

            foreach (Role person in title.ActingRoles)
                names.Add(person.PersonName);

            foreach (OMLEngine.Person person in title.Writers)
                names.Add(person.full_name);

            foreach (OMLEngine.Person person in title.Directors)
                names.Add(person.full_name);

            foreach (OMLEngine.Person person in title.Producers)
                names.Add(person.full_name);

            // now that we have all the people - query to see who exists
            var actors = from actor in context.BioDatas
                         where names.Contains(actor.FullName)
                         select actor;

            Dictionary<string, BioData> existingPeople = new Dictionary<string, BioData>();

            System.Diagnostics.Trace.WriteLine("Actors in current Context - " + actors.Count().ToString());
            
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
        public static Person CreatePerson(OMLDataDataContext context, string name, string characterName, PeopleRole role, Dictionary<string, BioData> existingPeople)
        {            
            BioData metaData = null;

            // see if the actor exists already in the in memory cache
            existingPeople.TryGetValue(name, out metaData);

            if (metaData == null)
            {
                // Ok, we may not have allready created but first check if sql thinks it has
                // SQL thinks 'æ' and 'ae' are so lets double check the database
                metaData = Dao.TitleCollectionDao.GetPersonBioDataByName(context, name);
            }

            if (metaData == null)
            {
                System.Diagnostics.Trace.WriteLine("Adding Bio for - " + name);
                // if it doesn't exist create a new one
                metaData = new BioData();
                metaData.FullName = name;
                context.BioDatas.InsertOnSubmit(metaData);
                context.SubmitChanges();

                // add the new metaData we added to the dictionary so we don't add it again
                existingPeople.Add(name, metaData);
            }
            else
            {
                System.Diagnostics.Trace.WriteLine("Found Bio for - " + name + " : Fullname - " + metaData.FullName);
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
