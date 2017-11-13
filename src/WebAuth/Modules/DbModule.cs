﻿using Autofac;
using AzureDataAccess;
using AzureDataAccess.AuditLog;
using AzureDataAccess.Email;
using AzureDataAccess.Settings;
using AzureRepositories.Assets;
using AzureStorage.Tables;
using Common.Log;
using Core.Application;
using Core.Assets.AssetGroup;
using Core.AuditLog;
using Core.BackOffice;
using Core.Bitcoin;
using Core.Clients;
using Core.Email;
using Core.Kyc;
using Core.Settings;
using Lykke.SettingsReader;

namespace WebAuth.Modules
{
    public class DbModule : Module
    {
        private readonly IReloadingManager<OAuthSettings> _settings;
        private readonly ILog _log;

        public DbModule(IReloadingManager<OAuthSettings> settings, ILog log)
        {
            _settings = settings;
            _log = log;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var clientPersonalInfoConnString = _settings.ConnectionString(x => x.OAuth.Db.ClientPersonalInfoConnString);
            var backOfficeConnString = _settings.ConnectionString(x => x.OAuth.Db.BackOfficeConnString);

            builder.RegisterInstance(
                new AuditLogRepository(AzureTableStorage<AuditLogDataEntity>.Create(clientPersonalInfoConnString, "AuditLogs", _log))
            ).As<IAuditLogRepository>().SingleInstance();

            builder.RegisterInstance(
                AzureRepoFactories.Applications.CreateApplicationsRepository(clientPersonalInfoConnString, _log)
            ).As<IApplicationRepository>().SingleInstance();

            builder.RegisterInstance(
                AzureRepoFactories.CreateTraderSettingsRepository(clientPersonalInfoConnString, _log)
            ).As<IClientSettingsRepository>().SingleInstance();

            builder.RegisterInstance(
                AzureRepoFactories.Clients.CreateTradersRepository(clientPersonalInfoConnString, _log)
            ).As<IClientAccountsRepository>().SingleInstance();
            
            builder.RegisterInstance(
                AzureRepoFactories.Clients.CreateKycRepository(clientPersonalInfoConnString, _log)
            ).As<IKycRepository>().SingleInstance();

            builder.RegisterInstance(
                AzureRepoFactories.Clients.CreateKycDocumentsRepository(clientPersonalInfoConnString, _log)
            ).As<IKycDocumentsRepository>().SingleInstance();

            builder.RegisterInstance(
                AzureRepoFactories.Clients.CreatKycDocumentsScansRepository(clientPersonalInfoConnString)
            ).As<IKycDocumentsScansRepository>().SingleInstance();

            builder.RegisterInstance(
                AzureRepoFactories.BackOffice.CreateMenuBadgesRepository(backOfficeConnString, _log)
            ).As<IMenuBadgesRepository>().SingleInstance();

            builder.RegisterInstance(
                new AssetGroupRepository(AzureTableStorage<AssetGroupEntity>.Create(clientPersonalInfoConnString, "AssetGroups", _log))
            ).As<IAssetGroupRepository>().SingleInstance();

            builder.RegisterInstance(
                new AppGlobalSettingsRepository(AzureTableStorage<AppGlobalSettingsEntity>.Create(clientPersonalInfoConnString, "Setup", _log))
            ).As<IAppGlobalSettingsRepositry>().SingleInstance();

            builder.RegisterInstance(
                AzureRepoFactories.Bitcoin.CreateWalletCredentialsRepository(clientPersonalInfoConnString, _log)
            ).As<IWalletCredentialsRepository>().SingleInstance();

            builder.RegisterInstance(
                new VerificationCodesRepository(AzureTableStorage<VerificationCodeEntity>.Create(clientPersonalInfoConnString, "VerificationCodes", _log))
            ).As<IVerificationCodesRepository>().SingleInstance();
        }
    }
}