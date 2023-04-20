using static System.Console;

namespace Momento;

public class Memento
{
    public Memento(int balance)
    {
        Balance = balance;
    }

    public int Balance { get; }
}

public class BankAccount
{
    private int _balance;
    private readonly List<Memento?> _changes = new();
    private int _current;

    public BankAccount(int balance)
    {
        this._balance = balance;
        _changes.Add(new Memento(balance));
    }

    public Memento? Deposit(int amount)
    {
        _balance += amount;
        var m = new Memento(_balance);
        _changes.Add(m);
        ++_current;
        return m;
    }

    public void Restore(Memento? m)
    {
        if (m != null)
        {
            _balance = m.Balance;
            _changes.Add(m);
            _current = _changes.Count - 1;
        }
    }

    public Memento? Undo()
    {
        if (_current > 0)
        {
            var m = _changes[--_current];
            _balance = m.Balance;
            return m;
        }

        return null;
    }

    public Memento? Redo()
    {
        if (_current + 1 < _changes.Count)
        {
            var m = _changes[++_current];
            _balance = m.Balance;
            return m;
        }

        return null;
    }

    public override string ToString()
    {
        return $"{nameof(_balance)}: {_balance}";
    }
}

public class Demo
{
    private static void Main(string[] args)
    {
        var ba = new BankAccount(100);
        ba.Deposit(50);
        ba.Deposit(25);
        WriteLine(ba);

        ba.Undo();
        WriteLine($"Undo 1: {ba}");
        ba.Undo();
        WriteLine($"Undo 2: {ba}");
        ba.Redo();
        WriteLine($"Redo 2: {ba}");
    }
}