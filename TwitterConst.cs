using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterSampleApp
{
    public static class TwitterConst
    {
        // CONSUMER_KEY , CONSUMER_SECRET の取得について
        // Application Management にて TwitterApps を作成して取得する
        // URL : https://apps.twitter.com/
        public static readonly string CONSUMER_KEY = "";
        public static readonly string CONSUMER_SECRET = "";

        public static readonly string REQUEST_TOKEN_URL = "https://api.twitter.com/oauth/request_token";
        public static readonly string AUTHORIZE_URL = "https://api.twitter.com/oauth/authorize";
        public static readonly string ACCESS_TOKEN_URL = "https://api.twitter.com/oauth/access_token";

    }
}
