using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osmo.Core.Objects
{
    class Skin
    {
        private string mName;
        private string mAuthor;
        private string mPath;
        VeryObservableCollection<string> mElements = new VeryObservableCollection<string>("Elements");

        #region Properties
        /// <summary>
        /// The visible name of this <see cref="Skin"/> object.
        /// </summary>
        public string Name { get => mName; set => mName = value; }

        /// <summary>
        /// The root folder of this <see cref="Skin"/> object.
        /// </summary>
        public string Path { get => mPath; set => mPath = value; }

        /// <summary>
        /// The root folder of this <see cref="Skin"/> object.
        /// </summary>
        public string Author { get => mAuthor; set => mAuthor = value; }

        /// <summary>
        /// This list contains all filenames of this <see cref="Skin"/> object.
        /// </summary>
        public VeryObservableCollection<string> Elements { get => mElements; set => mElements = value; }

        /// <summary>
        /// This returns the amount of elements this <see cref="Skin"/> object contains.
        /// </summary>
        public int ElementCount { get => mElements.Count; }
        #endregion

        public Skin(string path)
        {
        }

        public Skin()
        {
            mName = "Hello";
            mAuthor = "World!";
            for (int i = 0; i < 10; i++)
                mElements.Add("Element");
        }

        #region Method and operator overrides
        public static bool operator ==(Skin skin, string path)
        {
            if (path != null)
                return skin.Path.Contains(path);
            else
                return false;
        }

        public static bool operator !=(Skin skin, string path)
        {
            if (path != null)
                return !skin.Path.Contains(path);
            else
                return true;
        }

        public override bool Equals(object obj)
        {
            if (obj != null && Path != null && (obj as Skin).Path != null)
                return Path.Contains((obj as Skin).Path);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
