﻿namespace ListWithJson
{
    public static class Constants
    {
        public const string BaseAdress = "https://10.0.2.2:5001";
        public const string ApiProductsUrl = BaseAdress + "/api/Products/{0}";
        public const string ApiAuthenticationUrl = BaseAdress + "/api/User/{0}";
        public const string ApiFirebaseTokenRegistration = BaseAdress + "/api/User/register_firebase_token";

        public const string FirebaseTokenPreferenceTag = "firebase_registration_token";
    }
}