namespace Core;

public abstract class Value : Expression
{
    public Value Eval(Dictionary<int, Value> environment) => this;

    public virtual bool AsBool()
    {
        throw new EvaluationException("Not a Bool");
    }

    public virtual long AsInt()
    {
       throw new EvaluationException("Not an Int");
    }

    public virtual string AsMachineInt()
    {
        throw new EvaluationException("Not an Int");
    }

    public virtual string AsString()
    {
        throw new EvaluationException("Not a String");
    }

    public virtual string AsMachineString()
    {
        throw new EvaluationException("Not a String");
    }

    public abstract bool EqualsValue(Value other);
}
