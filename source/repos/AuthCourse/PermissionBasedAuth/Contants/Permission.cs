namespace PermissionBasedAuth.Contants
{
    public static class Permission
    {
        public static List<string> GenratePermissionList(string modelName)
        {
            return new List<string>
            {
                $"Permission.{modelName}.Create",
                $"Permission.{modelName}.Read",
                $"Permission.{modelName}.Update",
                $"Permission.{modelName}.Delete"
            };
        }
        public static class User
        {
            public const string Create = "Permission.User.Create";
            public const string Read = "Permission.User.Read";
            public const string Update = "Permission.User.Update";
            public const string Delete = "Permission.User.Delete";
        }
        public static class Role
        {
            public const string Create = "Permission.Role.Create";
            public const string Read = "Permission.Role.Read";
            public const string Update = "Permission.Role.Update";
            public const string Delete = "Permission.Role.Delete";
        }
    }
}
