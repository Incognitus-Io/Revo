﻿using System;
using MediatR;

namespace GTRevo.Platform.Events
{
    public interface IEvent : INotification
    {
        Guid Id { get; }
    }
}
