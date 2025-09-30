using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine.UI;

namespace Model
{
    public class Segment : IDisposable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event Action Disposed;
        
        public enum SegmentType
        {
            Floor = 0,
            Wall,
            Obstacle,
            
            MaxIndex,
        }

        public SegmentType Type
        {
            get => _type; 
            private set => SetField(ref _type, value);
        }

        public Segment(SegmentType type)
        {
            Type = type;
        }

        public void Dispose()
        {
            Disposed?.Invoke();
        }

        public void Toggle()
            => Type = (SegmentType)(((int)Type + 1) % (int)SegmentType.MaxIndex);
        
        private SegmentType _type;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}