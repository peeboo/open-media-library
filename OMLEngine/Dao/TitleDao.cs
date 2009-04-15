using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OMLEngine.Dao
{    
    internal static class TitleDao
    {
        private const char dbCollectionDelimiter = '\t';        

        public static void SetupCollectionsToBeAdded(OMLEngine.Title title)
        {
            Title daoTitle = title.DaoTitle;

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
            /*foreach (string name in title.Tags)
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
            }*/

            // grab from the db who we know about already
            Dictionary<string, BioData> existingPeople = GetAllExistingPeople(title);

            int actorIndex = 0;

            // add the actors
            foreach (string actor in title.ActingRoles.Keys)
            {
                Person person = CreatePerson(actor, title.ActingRoles[actor], PeopleRole.Actor, existingPeople);

                // maintain the order
                person.Sort = (short)(actorIndex++);

                // add them to the title
                daoTitle.People.Add(person);
            }

            // add the directors
            foreach (OMLEngine.Person director in title.Directors)
            {
                Person person = CreatePerson(director.full_name, null, PeopleRole.Director, existingPeople);

                // maintain the order
                person.Sort = (short)(actorIndex++);

                // add them to the title
                daoTitle.People.Add(person);
            }

            // add the writers
            foreach (OMLEngine.Person writer in title.Writers)
            {
                Person person = CreatePerson(writer.full_name, null, PeopleRole.Writer, existingPeople);

                // maintain the order
                person.Sort = (short)(actorIndex++);

                // add them to the title
                daoTitle.People.Add(person);
            }

            // add the producers
            foreach (OMLEngine.Person name in title.Producers)
            {
                Person person = CreatePerson(name.full_name, null, PeopleRole.Producers, existingPeople);

                // maintain the order
                person.Sort = (short)(actorIndex++);

                // add them to the title
                daoTitle.People.Add(person);
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
        private static Dictionary<string, BioData> GetAllExistingPeople(OMLEngine.Title title)
        {
            List<string> names = new List<string>(title.ActingRoles.Count + title.Writers.Count + title.Directors.Count + title.Producers.Count);

            foreach (string person in title.ActingRoles.Keys)
                names.Add(person);

            foreach (OMLEngine.Person person in title.Writers)
                names.Add(person.full_name);

            foreach (OMLEngine.Person person in title.Directors)
                names.Add(person.full_name);

            foreach (OMLEngine.Person person in title.Producers)
                names.Add(person.full_name);

            // now that we have all the people - query to see who exists
            var actors = from actor in DBContext.Instance.BioDatas
                         where names.Contains(actor.FullName)
                         select actor;

            Dictionary<string, BioData> existingPeople = new Dictionary<string, BioData>();

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
        public static Person CreatePerson(string name, string characterName, PeopleRole role, Dictionary<string, BioData> existingPeople)
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
