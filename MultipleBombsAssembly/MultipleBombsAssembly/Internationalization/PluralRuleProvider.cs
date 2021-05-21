using MultipleBombsAssembly.Internationalization.PluralRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultipleBombsAssembly.Internationalization
{
    public class PluralRuleProvider
    {
        public static IPluralRule GetPluralRuleFromLanguage(string language)
        {
            switch (language)
            {
                case "ak": // Akan
                case "bh":  // Bihari
                case "guw":  // Gun
                case "ln":  // Lingala
                case "mg":  // Malagasy
                case "nso":  // Northern Sotho
                case "pa":  // Punjabi
                case "ti":  // Tigrinya
                case "wa":  // Walloon
                    return new IntOneOrZeroPluralRule();
                case "am":  // Amharic
                case "bn":  // Bengali
                case "ff":  // Fulah
                case "gu":  // Gujarati
                case "hi":  // Hindi
                case "kn":  // Kannada
                case "mr":  // Marathi
                case "fa":  // Persian
                case "zu":  // Zulu
                    return new ZeroToOnePluralRule();
                case "hy":  // Armenian
                case "fr":  // French
                case "kab":  // Kabyle
                    return new ZeroToTwoExcludedPluralRule();

                /////////////////////////////////////////////////////////////////////////////////////////
                case "af":  // Afrikaans			
                case "sq":  // Albanian			
                case "ast": // Asturian			
                case "asa": // Asu			
                case "az":  // Azerbaijani			
                case "eu":  // Basque			
                case "bem": // Bemba			
                case "bez":  // Bena			
                case "brx":  // Bodo			
                case "bg":  // Bulgarian			
                case "ca":  // Catalan			
                case "chr":  // Cherokee			
                case "cgg":  // Chiga			

                case "dv":  // Divehi			
                case "nl":  // Dutch			
                case "en":  // English			
                case "eo":  // Esperanto			
                case "et":  // Estonian			
                case "pt":  // European Portuguese			
                case "ee":  // Ewe			
                case "fo":  // Faroese			
                case "fi":  // Finnish			
                case "fur":  // Friulian			
                case "gl":  // Galician			
                case "lg":  // Ganda			
                case "ka":  // Georgian			
                case "de":  // German			
                case "el":  // Greek			
                case "ha":  // Hausa			
                case "haw":  // Hawaiian			
                case "hu":  // Hungarian			
                case "it":  // Italian			
                case "kaj":  // Jju			
                case "kkj":  // Kako			
                case "kl":  // Kalaallisut			
                case "ks":  // Kashmiri			
                case "kk":  // Kazakh			
                case "ku":  // Kurdish			
                case "ky":  // Kyrgyz			
                case "lb":  // Luxembourgish			
                case "jmc":  // Machame			
                case "ml":  // Malayalam			
                case "mas":  // Masai			
                case "mgo":  // Meta'			
                case "mn":  // Mongolian			
                case "nah":  // Nahuatl			
                case "ne":  // Nepali			
                case "nnh":  // Ngiemboon			
                case "jgo":  // Ngomba			
                case "nd":  // North Ndebele			
                case "no":  // Norwegian			
                case "nb":  // Norwegian Bokmål			
                case "nn":  // Norwegian Nynorsk			
                case "ny":  // Nyanja			
                case "nyn":  // Nyankole			
                case "or":  // Oriya			
                case "om":  // Oromo			
                case "os":  // Ossetic			
                case "pap":  // Papiamento			
                case "ps":  // Pashto			
                case "rm":  // Romansh			
                case "rof":  // Rombo			
                case "rwk":  // Rwa			
                case "ssy":  // Saho			
                case "sag":  // Samburu			
                case "seh":  // Sena			
                case "ksb":  // Shambala			
                case "sn":  // Shona			
                case "xog":  // Soga			
                case "so":  // Somali			
                case "ckb":  // Sorani Kurdish			
                case "nr":  // South Ndebele			
                case "st":  // Southern Sotho			
                case "es":  // Spanish			
                case "sw":  // Swahili			
                case "ss":  // Swati			
                case "sv":  // Swedish			
                case "gsw":  // Swiss German			
                case "syr":  // Syriac			
                case "ta":  // Tamil			
                case "te":  // Telugu			
                case "teo":  // Teso			
                case "tig":  // Tigre			
                case "ts":  // Tsonga			
                case "tn":  // Tswana			
                case "tr":  // Turkish			
                case "tk":  // Turkmen			
                case "kcg":  // Tyap			
                case "ur":  // Urdu			
                case "ug":  // Uyghur			
                case "uz":  // Uzbek			
                case "ve":  // Venda			
                case "vo":  // Volapük			
                case "vun":  // Vunjo			
                case "wae":  // Walser			
                case "fy":  // Western Frisian			
                case "xh":  // Xhosa			
                case "yi":  // Yiddish			
                case "ji":  // ji
                    return new OnlyOnePluralRule();
                ////////////////////////////////////////////////////////////////////////////////////
                case "si": // Sinhala
                    return new SinhalaPluralRule();
                case "lv": // Latvian 
                case "prg": // Prussian
                    return new LatvianPluralRule();
                case "ga": // Irish
                    return new IrishPluralRule();
                case "ro": // Romanian
                case "mo":  // Moldavian
                    return new RomanianPluralRule();
                case "lt": // Lithuanian
                    return new LithuanianPluralRule();

                case "ru":  // Russian
                case "uk": // Ukrainian
                case "be": // Belarusian
                    return new SlavicPluralRule();
                case "cs": // Czech
                case "sk": // Slovak
                    return new CzechPluralRule();
                case "pl": // Polish 
                    return new PolishPluralRule();
                case "sl": // Slovenian
                    return new SlovenianPluralRule();
                // Arabic
                case "ar":
                    return new ArabicPluralRule();
                case "he": // Hebrew
                case "iw": // Iw
                    return new HebrewPluralRule();
                case "fil": // Filipino
                case "tl": // Tagalog
                    return new FilipinoPluralRule();
                case "mk":
                    return new MacedonianPluralRule();
                case "br": // Breton
                    return new BreizhPluralRule();
                case "tzm": // Central Atlas Tamazight
                    return new CentralAtlasTamazightPluralRule();
                case "ksh": //Colognian
                    return new OneOrZeroPluralRule();
                case "lag": //Langi
                    return new OneOrZeroToOneExcludedPluralRule();
                case "kw": // Cornish
                case "smn": // Inari Sami
                case "iu": // Inuktitut
                case "smj": // Lule Sami
                case "naq": // Nama
                case "se": // Northern Sami
                case "smi": // Sami languages [Other]
                case "sms": // Skolt Sami
                case "sma": // Southern Sami
                    return new OneOrTwoPluralRule();
                case "bs": // Bosnian
                case "hr": // Croatian
                case "sr": // Serbian
                case "sh": // Serbo-Croatian
                    return new CroatPluralRule();
                case "shi":
                    return new TachelhitPluralRule();
                case "is":
                    return new IcelandicPluralRule();
                case "gv":
                    return new ManxPluralRule();
                case "gd":
                    return new ScottishGaelicPluralRule();
                case "mt": // Maltese
                    return new MaltesePluralRule();
                case "cy": // Welsh
                    return new WelshPluralRule();
                case "da":  // Danish	
                    return new DanishPluralRule();
                default:
                    return new OtherPluralRule();
            }
        }
    }
}
