# GakkoServices.Core

This package contains any classes needed by more than one service, as well as
some helper classes.

## GakkoServices.Core.Messages

The classes that define messages used with RawRabbit. These classes must be
shared so both the producer/publisher and consumer/subscriber use the same class
with RawRabbit.

## GakkoServices.Core.Models

This namespace contains models used with the message queue. This makes it easier
to serialize/deserialize data that is send over the message queue.