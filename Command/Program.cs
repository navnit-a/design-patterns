using static System.Console;

namespace Command;

public class BankAccount
{
    private const int OverdraftLimit = -500;
    private int _balance;

    public void Deposit(int amount)
    {
        _balance += amount;
        WriteLine($"Deposited ${amount}, balance is now {_balance}");
    }

    public bool Withdraw(int amount)
    {
        if (_balance - amount >= OverdraftLimit)
        {
            _balance -= amount;
            WriteLine($"Withdrew ${amount}, balance is now {_balance}");
            return true;
        }

        return false;
    }

    public override string ToString()
    {
        return $"{nameof(_balance)}: {_balance}";
    }
}

public interface ICommand
{
    void Call();
    void Undo();
}

public class BankAccountCommand : ICommand
{
    public enum Action
    {
        Deposit,
        Withdraw
    }

    private readonly BankAccount _account;

    private readonly Action _action;
    private readonly int _amount;
    private bool _succeeded;

    public BankAccountCommand(BankAccount account, Action action, int amount)
    {
        _account = account ?? throw new ArgumentNullException(nameof(account));
        _action = action;
        _amount = amount;
    }

    public void Call()
    {
        switch (_action)
        {
            case Action.Deposit:
                _account.Deposit(_amount);
                _succeeded = true;
                break;
            case Action.Withdraw:
                _succeeded = _account.Withdraw(_amount);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Undo()
    {
        if (!_succeeded) return;
        switch (_action)
        {
            case Action.Deposit:
                _account.Withdraw(_amount);
                break;
            case Action.Withdraw:
                _account.Deposit(_amount);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

internal class Demo
{
    private static void Main(string[] args)
    {
        var ba = new BankAccount();
        var commands = new List<BankAccountCommand>
        {
            new(ba, BankAccountCommand.Action.Deposit, 100),
            new(ba, BankAccountCommand.Action.Withdraw, 1000)
        };

        WriteLine(ba);

        foreach (var c in commands)
            c.Call();

        WriteLine(ba);

        foreach (var c in Enumerable.Reverse(commands))
            c.Undo();

        WriteLine(ba);
    }
}