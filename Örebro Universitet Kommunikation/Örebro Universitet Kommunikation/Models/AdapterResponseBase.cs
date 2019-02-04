using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Örebro_Universitet_Kommunikation.Models {
    public class AdapterResponseBase {

        public class ResponseBase { }

        public class RequestBase { }

        public class RequestBase<T> : RequestBase {
            public RequestBase() { }
            public RequestBase(T data) {
                Data = data;
            }
            public T Data {
                get;
                set;
            }
        }
    }
}