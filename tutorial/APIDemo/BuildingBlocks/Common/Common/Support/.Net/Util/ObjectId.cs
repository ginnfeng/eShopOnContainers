namespace Common.Support.Net.Util
{
    public interface IObjectId
	{
        long Id { get;}
	}
    public class ObjectId : ObjectId<object>
    {
    }
    public class ObjectId<T> : IObjectId
	{
        static public  long NewId() { return globalId++;}
        public ObjectId()
        {
            Id = NewId();
        }
        public long Id { get; private set; }
       
        public override bool Equals(object it)
        {           
            if (it == null || GetType() != it.GetType())
            {
                return false;
            }
            return ((ObjectId<T>)it).Id == Id;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        static long globalId;
	}
}
