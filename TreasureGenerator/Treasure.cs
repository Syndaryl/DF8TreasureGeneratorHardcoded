using System;
using System.Runtime.Serialization;

namespace Syndaryl.TreasureGenerator
{
    internal class Treasure
    {
        string libraryName = "";
        private df8 library;

        public Treasure(string LibraryName)
        {
            this.LibraryName = LibraryName;
        }

        public string LibraryName
        {
            get
            {
                return libraryName;
            }

            set
            {
                libraryName = value;
                Library = df8.LoadFile(libraryName);
            }
        }

        public df8 Library
        {
            get
            {
                return library;
            }

            set
            {
                library = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="LibraryName">File name/path of the library XML to load. Must conform to UnifiedFile.xsd so it can be deserialized.</param>
        /// <returns></returns>
        internal object Generate(string LibraryName)
        {
            if (!this.LibraryName.Equals(LibraryName) || Library == null)
            {
                this.LibraryName = LibraryName;
            }
            return Generate();
        }

        internal object Generate()
        {
            if (Library == null || this.LibraryName.Equals(string.Empty))
            {
                throw new NoLibraryException("Tried to generate a random item without a library successfully loaded!");
            }

            throw new NotImplementedException();
        }
    }

    [Serializable]
    internal class NoLibraryException : Exception
    {
        public NoLibraryException()
        {
        }

        public NoLibraryException(string message) : base(message)
        {
        }

        public NoLibraryException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoLibraryException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}