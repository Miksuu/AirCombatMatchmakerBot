﻿using System.Collections.Concurrent;
using System.Runtime.Serialization;

[DataContract]
public class DeleteChannelEvent : ScheduledEvent
{
    public string NameMustContain
    {
        get => nameMustContain.GetValue();
        set => nameMustContain.SetValue(value);
    }

    [DataMember] private logString nameMustContain = new logString();

    public DeleteChannelEvent() { }

    public DeleteChannelEvent(
        ulong _timeFromNowToExecuteOn, ulong _categoryIdToDeleteChannelOn, ulong _channelIdToDelete, string _nameMustContain,
        ConcurrentBag<ScheduledEvent> _scheduledEvents)
    {
        Log.WriteLine("Creating event: " + nameof(DeleteChannelEvent) + " with: " + _timeFromNowToExecuteOn + "|" +
            _categoryIdToDeleteChannelOn + "|" + _channelIdToDelete + "|" + _nameMustContain);

        base.SetupScheduledEvent(_timeFromNowToExecuteOn, _scheduledEvents);
        LeagueCategoryIdCached = _categoryIdToDeleteChannelOn;
        MatchChannelIdCached = _channelIdToDelete;
        if (_nameMustContain == "")
        {
            Log.WriteLine("nameMustContain was empty!", LogLevel.ERROR);
            _nameMustContain = "WontDelete";
        }
        NameMustContain = _nameMustContain;

        Log.WriteLine("Done creating event: " + nameof(DeleteChannelEvent) + " with: " + _timeFromNowToExecuteOn + "|" +
            _categoryIdToDeleteChannelOn + "|" + _channelIdToDelete + "|" + _nameMustContain, LogLevel.DEBUG);
    }

    // Event that will be executed instantly
    public DeleteChannelEvent(ulong _categoryIdToDeleteChannelOn, ulong _channelIdToDelete, string _nameMustContain)
    {
        Log.WriteLine("Creating instantly executable event: " + nameof(DeleteChannelEvent) + " with: " + "|" +
            _categoryIdToDeleteChannelOn + "|" + _channelIdToDelete + "|" + _nameMustContain);

        //base.SetupScheduledEvent(_timeFromNowToExecuteOn);
        LeagueCategoryIdCached = _categoryIdToDeleteChannelOn;
        MatchChannelIdCached = _channelIdToDelete;
        if (_nameMustContain == "")
        {
            Log.WriteLine("nameMustContain was empty!", LogLevel.ERROR);
            _nameMustContain = "WontDelete";
        }
        NameMustContain = _nameMustContain;

        Log.WriteLine("Done creating instantly executable event: " + nameof(DeleteChannelEvent) + " with: " +
            _categoryIdToDeleteChannelOn + "|" + _channelIdToDelete + "|" + _nameMustContain, LogLevel.DEBUG);
    }

    public override async Task ExecuteTheScheduledEvent(bool _serialize = true)
    {
        try
        {
            ulong categoryId = LeagueCategoryIdCached;
            ulong channelId = MatchChannelIdCached;
            string nameMustContain = NameMustContain;

            Log.WriteLine("Starting to execute event: " + EventId + " named " + nameof(DeleteChannelEvent) + " with: " +
                categoryId + "|" + channelId + "|" + nameMustContain);

            InterfaceCategory interfaceCategory =
                Database.GetInstance<DiscordBotDatabase>().Categories.FindInterfaceCategoryWithCategoryId(categoryId);

            Log.WriteLine("Event: " + EventId + " before " +
                nameof(interfaceCategory.FindIfInterfaceChannelExistsWithIdInTheCategory));

            if (interfaceCategory.FindIfInterfaceChannelExistsWithIdInTheCategory(channelId))
            {
                InterfaceChannel interfaceChannel;


                Log.WriteLine("Event: " + EventId + " inside " +
                    nameof(interfaceCategory.FindIfInterfaceChannelExistsWithIdInTheCategory));

                interfaceChannel = interfaceCategory.FindInterfaceChannelWithIdInTheCategory(channelId);


                Log.WriteLine("Event: " + EventId + " found: " + interfaceChannel.ChannelName);
                await interfaceChannel.DeleteThisChannel(interfaceCategory, interfaceChannel, nameMustContain);
                Log.WriteLine("Event: " + EventId + " after deletion of: " + interfaceChannel.ChannelName);
            }
            else
            {
                //Log.WriteLine("Finished an event without deleting the channel, because it didn't exist!", LogLevel.WARNING);
            }

            Log.WriteLine("Done executing event: " + nameof(DeleteChannelEvent) + " with: " +
                categoryId + "|" + channelId + "|" + nameMustContain, LogLevel.DEBUG);

            if (!_serialize) return;

            await SerializationManager.SerializeDB();
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.ERROR);
            return;
        }
    }

    public override void CheckTheScheduledEventStatus()
    {
        try
        {
            Database.GetInstance<DiscordBotDatabase>().Categories.FindInterfaceCategoryWithCategoryId(
                LeagueCategoryIdCached).FindInterfaceChannelWithIdInTheCategory(
                    MatchChannelIdCached).FindInterfaceMessageWithNameInTheChannelAndUpdateItIfItExists(
                        MessageName.CONFIRMATIONMESSAGE);

        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.ERROR);
            return;
        }
    }
}