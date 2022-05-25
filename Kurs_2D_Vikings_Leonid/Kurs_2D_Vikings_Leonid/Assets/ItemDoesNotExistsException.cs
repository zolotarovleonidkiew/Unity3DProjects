using System;

/// <summary>
/// Inventory exception - ItemDoesNotExistsException
/// </summary>
public class ItemDoesNotExistsException : Exception
{
    public ItemDoesNotExistsException() : base()
    {
        
    }

    public ItemDoesNotExistsException(string message) : base(message)
    {

    }

    public ItemDoesNotExistsException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
