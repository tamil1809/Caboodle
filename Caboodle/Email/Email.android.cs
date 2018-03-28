﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Text;

using AndroidUri = Android.Net.Uri;
using JavaFile = Java.IO.File;

namespace Microsoft.Caboodle
{
    public static partial class Email
    {
        static readonly EmailMessage testEmail =
            new EmailMessage("Testing Caboodle", "This is a test email.", "caboodle.example.org");

        internal static bool IsComposeSupported
            => Platform.IsIntentSupported(CreateIntent(testEmail));

        static Task PlatformComposeAsync(EmailMessage message)
        {
            var intent = CreateIntent(message)
                .SetFlags(ActivityFlags.ClearTop)
                .SetFlags(ActivityFlags.NewTask);

            Platform.CurrentContext.StartActivity(intent);

            return Task.FromResult(true);
        }

        static Intent CreateIntent(EmailMessage message)
        {
            var intent = new Intent(Intent.ActionSend);
            intent.SetType("message/rfc822");

            if (!string.IsNullOrEmpty(message?.Body))
            {
                if (message?.BodyFormat == EmailBodyFormat.Html)
                {
                    ISpanned html;
                    if (Platform.HasApiLevel(BuildVersionCodes.N))
                    {
                        html = Html.FromHtml(message.Body, FromHtmlOptions.ModeLegacy);
                    }
                    else
                    {
#pragma warning disable CS0618 // Type or member is obsolete
                        html = Html.FromHtml(message.Body);
#pragma warning restore CS0618 // Type or member is obsolete
                    }
                    intent.PutExtra(Intent.ExtraText, html);
                }
                else
                {
                    intent.PutExtra(Intent.ExtraText, message.Body);
                }
            }
            if (!string.IsNullOrEmpty(message?.Subject))
                intent.PutExtra(Intent.ExtraSubject, message.Subject);
            if (message.To?.Count > 0)
                intent.PutExtra(Intent.ExtraEmail, message.To.ToArray());
            if (message.Cc?.Count > 0)
                intent.PutExtra(Intent.ExtraCc, message.Cc.ToArray());
            if (message.Bcc?.Count > 0)
                intent.PutExtra(Intent.ExtraBcc, message.Bcc.ToArray());

            if (message?.Attachments?.Count > 0)
            {
                // TODO: we may want to use FileProvider
                // intent.AddFlags(ActivityFlags.GrantReadUriPermission);

                var uris = new List<IParcelable>();
                foreach (var attachment in message.Attachments)
                {
                    // TODO: we may want to use FileProvider
                    // var file = new JavaFile(attachment.FilePath);
                    // uris.Add(FileProvider.GetUriForFile(Platform.CurrentContext, $"{AppInfo.PackageName}.fileprovider", file));

                    uris.Add(AndroidUri.Parse($"file://{attachment.FilePath}"));
                }
                intent.PutParcelableArrayListExtra(Intent.ExtraStream, uris);
            }

            return intent;
        }
    }
}
