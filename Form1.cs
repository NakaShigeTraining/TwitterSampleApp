using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// 参考URL
// http://kuroeveryday.blogspot.jp/2013/04/ctwitteroauth.html
// http://oppaoppa.hotcom-web.com/twcli/3.php

namespace TwitterSampleApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //---------------------------
            // 0.リクエストトークン取得の前処理
            //---------------------------

            OAuthBase oauth = new OAuthBase();

            // ランダム文字列の生成
            string nonce = oauth.GenerateNonce();

            // タイムスタンプ（unix時間）
            string timestamp = oauth.GenerateTimeStamp();
            string normalizedUrl, normalizedReqParams;

            Uri reqUrl = new Uri(TwitterConst.REQUEST_TOKEN_URL);

            // Consumer_Secretを暗号鍵とした署名の生成
            string signature = oauth.GenerateSignature(reqUrl
                                                    , TwitterConst.CONSUMER_KEY
                                                    , TwitterConst.CONSUMER_SECRET
                                                    , null
                                                    , null
                                                    , "GET"
                                                    , timestamp
                                                    , nonce
                                                    , OAuthBase.SignatureTypes.HMACSHA1
                                                    , out normalizedUrl
                                                    , out normalizedReqParams);

            // リクエストトークン取得用URL
            string reqTokenUrl = normalizedUrl + "?"
                               + normalizedReqParams
                               + "&oauth_signature=" + signature;

            //MessageBox.Show(reqTokenUrl);


            //---------------------------
            // 1.リクエストトークン取得
            //---------------------------

            WebClient client = new WebClient();
            Stream st = client.OpenRead(reqTokenUrl);
            StreamReader sr = new StreamReader(st, Encoding.GetEncoding("Shift_JIS"));

            var tokens = convertToTokenForOauth(sr.ReadToEnd());

            // 取得したリクエストトークン
            Console.WriteLine(
                  "(request)oauth_token        = {0}\r\n"
                + "(requrst)oauth_token_secret = {1}\r\n"
                , tokens["oauth_token"]
                , tokens["oauth_token_secret"]
                 );



            //---------------------------
            // 2.オーサライズ
            //---------------------------

            string authorizeUrl = TwitterConst.AUTHORIZE_URL + "?"
                                    + "oauth_token=" + tokens["oauth_token"]
                                    + "&oauth_token_secret=" + tokens["oauth_token_secret"];

            // ブラウザ起動しPINコードを表示
            System.Diagnostics.Process.Start(authorizeUrl);


            //---------------------------
            // 4.アクセストークン取得
            //---------------------------

            // リクエストトークンを加えsignatureを再生成
            signature = oauth.GenerateSignature(reqUrl
                                                , TwitterConst.CONSUMER_KEY
                                                , TwitterConst.CONSUMER_SECRET
                                                , tokens["oauth_token"]
                                                , tokens["oauth_token_secret"]
                                                , "GET"
                                                , timestamp
                                                , nonce
                                                , OAuthBase.SignatureTypes.HMACSHA1
                                                , out normalizedUrl
                                                , out normalizedReqParams);


            // アクセストークン取得用URL
            string accessTokenUrl = TwitterConst.ACCESS_TOKEN_URL + "?"
                                        + normalizedReqParams
                                        + "&oauth_signature=" + signature
                                        + "&oauth_verifier=" + pin;

            st = client.OpenRead(accessTokenUrl);
            sr = new StreamReader(st, Encoding.GetEncoding("Shift_JIS"));

            tokens = convertToTokenForOauth(sr.ReadToEnd());

            // 取得したアクセストークン
            Console.WriteLine(
                  "(access)oauth_token         = {0}\r\n"
                + "(access)oauth_token_secret  = {1}\r\n"
                + "user_id                     = {2}\r\n"
                + "screen_name                 = {3}\r\n"
                , tokens["oauth_token"]
                , tokens["oauth_token_secret"]
                , tokens["user_id"]
                , tokens["screen_name"]
                 );
        }

        // 取得した文字列を分解し、ハッシュテーブルに格納する
        private Dictionary<string, string> convertToTokenForOauth(string data)
        {
            var oauthKey = new System.Collections.Generic.Dictionary<string, string>();

            foreach (string s in data.Split('&'))
            {
                oauthKey.Add(s.Substring(0, s.IndexOf("=")), s.Substring(s.IndexOf("=") + 1));
            }

            return oauthKey;
        }
    }
}
