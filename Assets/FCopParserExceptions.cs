using System;
using System.Runtime.Serialization;

namespace FCopParser {

    public class InvalidFileException : Exception {

        public InvalidFileException() {
        }

    }

    public class MeshIDException : Exception {

        public MeshIDException() {
        }

    }

    public class TextureArrayMaxExceeded : Exception {

        public TextureArrayMaxExceeded() {
        }

    }

    public class GraphicsArrayMaxExceeded : Exception {

        public GraphicsArrayMaxExceeded() {
        }

    }

}