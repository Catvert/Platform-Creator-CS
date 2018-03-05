namespace Platform_Creator_CS.Entities {
    public enum Tags {
        Empty,
        Sprite,
        Player,
        Enemy,
        Special
    }

    public static class TagsExtension {
        public static string GetTag(this Tags tag) {
            return tag.ToString().ToLower();
        }
    }
}