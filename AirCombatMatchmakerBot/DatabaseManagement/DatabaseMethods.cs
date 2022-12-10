public static class DatabaseMethods
{
    // Just checks if the User discord ID profile exists in the database file
    public static bool CheckIfUserIdExistsInTheDatabase(ulong _id)
    {
        return Database.Instance.PlayerData.PlayerIDs.ContainsKey(_id);
    }
}