namespace Utils
{
    public static class Conditional
    {
        public static void If(bool condition, System.Action action) => (condition ? action : () => { })();
        public static void IfElse(bool condition, System.Action ifAction, System.Action elseAction) => (condition ? ifAction : elseAction)();
    }
}