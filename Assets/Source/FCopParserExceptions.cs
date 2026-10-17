using System;

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

    public class ColorArrayMaxExceeded : Exception { 
        public ColorArrayMaxExceeded() {

        } 
    }

    public class MaxTilesExceeded : Exception {

        public MaxTilesExceeded() {

        }

    }

}