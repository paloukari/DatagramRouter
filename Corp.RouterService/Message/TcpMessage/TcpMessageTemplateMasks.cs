
using System;
namespace Corp.RouterService.Message
{


    public static class HeaderTemplateConsts
    {
        public const char HeaderTextCharacter = 'H';         
        public const char HeaderPrefixCharacter = 'P';
        public const char HeaderSuffixCharacter = 'S';
        public const char HeaderLengthIndicatorCharacter = 'L';
        public const char HeaderWildcardCharacter = '*';
        public const char HeaderCopyBytesCharacter = 'C';
    }

    public enum TcpMessageTemplateMasks : byte
    {
        HeaderText = (byte)HeaderTemplateConsts.HeaderTextCharacter,
        HeaderPrefix = (byte)HeaderTemplateConsts.HeaderPrefixCharacter,
        HeaderSuffix = (byte)HeaderTemplateConsts.HeaderSuffixCharacter,
        HeaderLengthIndicator = (byte)HeaderTemplateConsts.HeaderLengthIndicatorCharacter,
        HeaderCopyBytes = (byte)HeaderTemplateConsts.HeaderCopyBytesCharacter,
        HeaderWildcard = (byte)HeaderTemplateConsts.HeaderWildcardCharacter
    }
}
