

using System.Collections.Generic;

namespace FCopParser {

    public class FCopScript {

        public int offset;
        public List<byte> compiledBytes = new();

        public FCopScript(int offset, List<byte> compiledBytes) {
            this.offset = offset;
            this.compiledBytes.AddRange(compiledBytes);
        }

    }


}