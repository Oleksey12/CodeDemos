using System;
using System.Collections.Generic;
using System.Linq;


namespace Assets.Project.Scripts.Abstract {
    public interface IObserver {
        public void Update(object value);
    }
    public interface ISubject {
        public void Attach(IObserver observer);
        public void Dettach(IObserver observer);
        public void Notify();


    }

    public interface FloatProperty {
        public bool ChangeValue(float newValue);
        public bool IncreaseValue(float diff);
        public float GetValue();
    }

    public interface IntProperty {
        public bool ChangeValue(int newValue);
        public bool IncreaseValue(int diff);
        public int GetValue();
    }
    public interface StringProperty {
        public bool ChangeValue(string newValue);
        public string GetValue();
    }


    public class FloatObserveredProperty: ISubject, FloatProperty {
        protected float _val = 0;
        protected List<IObserver> _observers = new List<IObserver>();
        public FloatObserveredProperty() { }
        public FloatObserveredProperty(float val) { _val = val; }

        public virtual bool IncreaseValue(float diff) {
            _val = _val + diff;
            Notify();

            return true;
        }
        public virtual bool ChangeValue(float newValue) {
            _val = newValue;
            Notify();
            return true;
        }   

        public virtual float GetValue() {
            return _val;
        }

        public void Attach(IObserver observer) {
            _observers.Append(observer);
        }

        public void Dettach(IObserver observer) {
            _observers.Remove(observer);
        }

        public void Notify() {
            foreach (IObserver observer in _observers) {
                observer.Update(_val);
            }
        }
    }
    public class IntObserveredProperty : ISubject, IntProperty {
        protected int _val = 0;
        protected List<IObserver> _observers = new List<IObserver>();
        public IntObserveredProperty(int val) { _val = val; }
        public IntObserveredProperty() {  }
        public virtual bool IncreaseValue(int diff) {
            _val = _val + diff;
            Notify();

            return true;
        }
        public virtual bool ChangeValue(int newValue) {
            _val = newValue;
            Notify();
            return true;
        }

        public virtual int GetValue() {
            return _val;
        }

        public void Attach(IObserver observer) {
            _observers.Append(observer);
        }

        public void Dettach(IObserver observer) {
            _observers.Remove(observer);
        }

        public void Notify() {
            foreach (IObserver observer in _observers) {
                observer.Update(_val);
            }
        }
    }
    public class StringObserveredProperty : ISubject, StringProperty {
        protected string _val;
        protected List<IObserver> _observers = new List<IObserver>();

        public StringObserveredProperty() { }
        public StringObserveredProperty(string val) { _val = val; }

        public virtual bool ChangeValue(string newValue) {
            _val = newValue;
            Notify();
            return true;
        }

        public virtual string GetValue() {
            return _val;
        }

        public void Attach(IObserver observer) {
            _observers.Append(observer);
        }

        public void Dettach(IObserver observer) {
            _observers.Remove(observer);
        }

        public void Notify() {
            foreach (IObserver observer in _observers) {
                observer.Update(_val);
            }
        }
    }
    

    public abstract class ProtectedFloatProperty : FloatObserveredProperty {
        protected virtual bool CheckFunction(float newValue) {
            return true;
        }

        protected virtual void ExceptionFunction() {
            throw new Exception("ValueError");
        }

        public override bool IncreaseValue(float diff) {
            bool res = CheckFunction(_val + diff);
            if (res) {
                base.IncreaseValue(diff);
            }
            ExceptionFunction();
            return res;
        }
        public override bool ChangeValue(float newValue) {
            bool res = CheckFunction(newValue);
            if (res) {
                return base.ChangeValue(newValue);
            }
            ExceptionFunction();
            return false;
        }
    }
    public abstract class ProtectedIntProperty : IntObserveredProperty {

        protected virtual bool CheckFunction(int newValue) {
            return true;
        }

        protected virtual void ExceptionFunction() {
            throw new Exception("ValueError");
        }

        public override bool IncreaseValue(int diff) {
            bool res = CheckFunction(_val + diff);
            if (res) {
                return base.IncreaseValue(diff);
            }
            ExceptionFunction();
            return res;
        }

        public override bool ChangeValue(int newValue) {
            bool res = CheckFunction(newValue);
            if (res) {
                return base.ChangeValue(newValue);
            }
            ExceptionFunction();
            return false;
        }
    }
    public abstract class ProtectedStringProperty : StringObserveredProperty {
        protected virtual bool CheckFunction(string newValue) {
            return newValue != "";
        }

        protected virtual void ExceptionFunction() {
            throw new Exception("ValueError");
        }

        public override bool ChangeValue(string newValue) {
            bool res = CheckFunction(newValue);
            if (res) {
                return base.ChangeValue(newValue);
            }
            ExceptionFunction();
            return false;
        }
    }
    
}
