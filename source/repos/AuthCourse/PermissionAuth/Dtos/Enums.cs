namespace PermissionAuth.Dtos
{
    public enum Role
    {
        Admin = 0,
        Manager,
        User
    }


    public enum Permission
    {
        ProductCreate = 0,
        ProductRead,
        ProductUpdate,
        ProductDelete,

        OrderCreate,
        OrderRead,
        OrderUpdate,
        OrderDelete,

    }

}
