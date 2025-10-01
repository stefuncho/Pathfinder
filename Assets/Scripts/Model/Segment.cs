using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Model
{
    public class Segment : IDisposable, INotifyPropertyChanged
    {
        public enum SegmentType
        {
            Floor = 0,
            Wall,
            Obstacle,
            
            MaxIndex,
        }
        
        public enum ColorType
        {
            Normal = 0,
            InRange,
            OutOfRange,
            
            MaxIndex,
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        public event Action Disposed;

        public SegmentType Type
        {
            get => _type; 
            private set => SetField(ref _type, value);
        }

        public ColorType Color
        {
            get => _color; 
            set => SetField(ref _color, value);
        }

        public Vector2Int Position { get; }

        public Segment(SegmentType type, Vector2Int position)
        {
            Type = type;
            Position = position;
        }

        public void Dispose()
        {
            Disposed?.Invoke();
        }

        public void Toggle()
            => Type = (SegmentType)(((int)Type + 1) % (int)SegmentType.MaxIndex);
        
        private SegmentType _type;
        private ColorType _color;

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